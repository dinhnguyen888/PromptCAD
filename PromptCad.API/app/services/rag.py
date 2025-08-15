import os
import faiss
from langchain_google_genai import GoogleGenerativeAIEmbeddings
from langchain_community.vectorstores import FAISS
from langchain.text_splitter import CharacterTextSplitter
from langchain.docstore.document import Document

class RAGService:
    def __init__(self, api_key: str, docs_dir: str = "app/documents"):
        self.api_key = api_key
        self.docs_dir = docs_dir
        self.vector_store = self._create_vector_store()

    def _create_vector_store(self):
        """Tạo hoặc load vector database"""
        embeddings = GoogleGenerativeAIEmbeddings(
            model="models/embedding-001",
            google_api_key=self.api_key
        )
        text_splitter = CharacterTextSplitter(chunk_size=1000, chunk_overlap=100)

        documents = []
        for file in os.listdir(self.docs_dir):
            if file.endswith(".txt"):
                with open(os.path.join(self.docs_dir, file), "r", encoding="utf-8") as f:
                    text = f.read()
                    chunks = text_splitter.split_text(text)
                    for chunk in chunks:
                        documents.append(Document(page_content=chunk))

        return FAISS.from_documents(documents, embeddings)

    def retrieve_context(self, query: str, k: int = 3) -> str:
        """Truy vấn context từ vector DB"""
        retriever = self.vector_store.as_retriever(search_kwargs={"k": k})
        results = retriever.get_relevant_documents(query)
        return "\n".join([doc.page_content for doc in results])