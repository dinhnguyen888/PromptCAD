import sys
import os
from getpass import getpass
from datetime import datetime

from dotenv import load_dotenv
from pymongo import MongoClient
from pymongo.errors import DuplicateKeyError

from app.core.security import hash_password


def prompt(text: str, default: str | None = None) -> str:
    suffix = f" [{default}]" if default else ""
    value = input(f"{text}{suffix}: ").strip()
    return value or (default or "")


def main() -> None:
    load_dotenv()

    # Read connection info from environment (no console input)
    mongo_uri = os.getenv("MONGO_URL", "mongodb://localhost:27017")
    db_name = os.getenv("MONGODB_DB", "promptcad_db")

    try:
        client = MongoClient(mongo_uri, serverSelectionTimeoutMS=10_000)
        client.admin.command("ping")
    except Exception as exc:
        print(f"[ERROR] Cannot connect to MongoDB at {mongo_uri}: {exc}")
        sys.exit(1)

    db = client[db_name]
    accounts = db["accounts"]

    # Ensure unique index on email
    try:
        accounts.create_index("email", unique=True)
    except Exception as exc:
        print(f"[WARN] Could not create index on 'email': {exc}")

    print("\n== Create/Update Admin Account ==")
    email = prompt("Admin email")
    if not email:
        print("[ERROR] Email is required.")
        sys.exit(1)

    while True:
        password = getpass("Admin password: ")
        if not password:
            print("Password cannot be empty.")
            continue
        confirm = getpass("Confirm password: ")
        if password != confirm:
            print("Passwords do not match. Try again.\n")
            continue
        break

    password_hash = hash_password(password)
    now = datetime.utcnow()

    # Upsert admin account
    try:
        res = accounts.update_one(
            {"email": email},
            {
                "$set": {"email": email, "password_hash": password_hash, "role": "admin"},
                "$setOnInsert": {"created_at": now},
            },
            upsert=True,
        )
    except DuplicateKeyError:
        print("[ERROR] Email already exists with a different account.")
        sys.exit(1)

    if res.upserted_id is not None:
        print(f"[OK] Admin account created for {email}.")
    else:
        print(f"[OK] Admin account updated for {email}.")


if __name__ == "__main__":
    main()


