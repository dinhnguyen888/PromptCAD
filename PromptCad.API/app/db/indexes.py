from app.db.mongo import get_collection


async def ensure_indexes() -> None:
    """Create required MongoDB indexes. Safe to call at startup.

    The function is resilient: callers should handle exceptions to avoid
    preventing the application from starting when MongoDB is unavailable.
    """
    # Accounts
    accounts = get_collection("accounts")
    await accounts.create_index("email", unique=True)

    # API keys: unique key and TTL cleanup a few days after expiry
    api_keys = get_collection("api_keys")
    await api_keys.create_index("key", unique=True)
    # Auto delete API keys 3 days after their expires_at
    await api_keys.create_index("expires_at", expireAfterSeconds=3 * 24 * 3600)

    # Admin tokens should expire exactly at expires_at
    admin_tokens = get_collection("admin_tokens")
    await admin_tokens.create_index("expires_at", expireAfterSeconds=0)


