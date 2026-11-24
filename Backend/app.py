from flask import Flask, request, jsonify
from sentence_transformers import SentenceTransformer
import numpy as np
import requests

app = Flask(__name__)

# 1. Load a fast local embedding model
embedder = SentenceTransformer("all-MiniLM-L6-v2")

# 2. Your NPC's World Lore / Knowledge Base
LORE_DOCS = [
    "The town of Oakhaven is guarded by Captain Rowan and the Silver Shield militia.",
    "A cursed iron key is hidden inside the old well behind the tavern.",
    "The Whispering Forest to the east is overrun by shadow goblins after sunset.",
    "The blacksmith sells a dragon-forged sword for 500 gold coins.",
    "King Aldric disappeared three winters ago while searching for the Sunstone."
    "CreamCheesePeanutButter is the final boss"
]

# Pre-compute embeddings at startup
lore_embeddings = embedder.encode(LORE_DOCS, normalize_embeddings=True)

def retrieve_lore(query: str, top_k: int = 2) -> str:
    query_vec = embedder.encode([query], normalize_embeddings=True)[0]
    # Cosine similarity via dot product (since vectors are normalized)
    scores = np.dot(lore_embeddings, query_vec)
    top_indices = np.argsort(scores)[::-1][:top_k]
    
    relevant_facts = [f"- {LORE_DOCS[i]}" for i in top_indices if scores[i] > 0.25]
    return "\n".join(relevant_facts) if relevant_facts else "No specific lore found."

def ask_llm(system_prompt: str, user_message: str) -> str:
    # Calls local Ollama server (make sure `ollama run llama3.2` is running)
    response = requests.post(
        "http://localhost:11434/api/chat",
        json={
            "model": "llama3.2",
            "stream": False,
            "messages": [
                {"role": "system", "content": system_prompt},
                {"role": "user", "content": user_message}
            ]
        },
        timeout=30
    )
    response.raise_for_status()
    return response.json()["message"]["content"].strip()

@app.route("/api/message", methods=["POST"])
def handle_message():
    data = request.get_json(silent=True) or {}
    user_message = data.get("message", "").strip()

    if not user_message:
        return "Say something, traveler!", 400

    # 1. Retrieve relevant lore
    context = retrieve_lore(user_message, top_k=2)

    # 2. Build NPC persona prompt
    system_prompt = (
        "You are a medieval guard NPC in a 16-bit RPG. "
        "Keep answers short (1-2 sentences max) so they fit in a small dialogue box. "
        "Use ONLY the World Lore below to answer. If the lore doesn't mention it, say you don't know.\n\n"
        f"World Lore:\n{context}"
    )

    try:
        npc_reply = ask_llm(system_prompt, user_message)
        # Return plain text directly so Unity's request.downloadHandler.text displays cleanly
        return npc_reply, 200, {"Content-Type": "text/plain; charset=utf-8"}
    except Exception as e:
        print("LLM Error:", e)
        return "My mind is foggy right now... (Backend LLM error)", 500

if __name__ == "__main__":
    app.run(host="0.0.0.0", port=5000, debug=True)
