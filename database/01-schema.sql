-- ====================================
-- AppChat Database Schema
-- Created automatically for Docker init
-- ====================================

-- Create Users table
CREATE TABLE IF NOT EXISTS "Users" (
    "Id" SERIAL PRIMARY KEY,
    "FirstName" VARCHAR(100) NOT NULL,
    "LastName" VARCHAR(100) NOT NULL,
    "PhoneNumber" VARCHAR(20) UNIQUE NOT NULL,
    "Password" VARCHAR(255) NOT NULL,
    "AvatarUrl" VARCHAR(500) DEFAULT '',
    "IsOnline" BOOLEAN DEFAULT false,
    "LastSeen" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Create Chats table
CREATE TABLE IF NOT EXISTS "Chats" (
    "Id" SERIAL PRIMARY KEY,
    "UserAId" INTEGER NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "UserBId" INTEGER NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "LastMessage" VARCHAR(500) DEFAULT '',
    "LastMessageTime" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "UnreadCount" INTEGER DEFAULT 0,
    CONSTRAINT unique_chat UNIQUE ("UserAId", "UserBId")
);

-- Create Messages table
CREATE TABLE IF NOT EXISTS "Messages" (
    "Id" SERIAL PRIMARY KEY,
    "ChatId" INTEGER NOT NULL REFERENCES "Chats"("Id") ON DELETE CASCADE,
    "SenderId" INTEGER NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "Content" TEXT,
    "FileUrl" VARCHAR(500),
    "FileType" VARCHAR(50),
    "Status" VARCHAR(50) DEFAULT '',
    "SentAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Create Contacts (Friends) table
CREATE TABLE IF NOT EXISTS "Contacts" (
    "Id" SERIAL PRIMARY KEY,
    "UserIdContactA" INTEGER NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "UserIdContactB" INTEGER NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    CONSTRAINT unique_contact UNIQUE ("UserIdContactA", "UserIdContactB")
);

-- Create indexes for better query performance
CREATE INDEX IF NOT EXISTS idx_chats_users ON "Chats"("UserAId", "UserBId");
CREATE INDEX IF NOT EXISTS idx_messages_chat ON "Messages"("ChatId");
CREATE INDEX IF NOT EXISTS idx_messages_sender ON "Messages"("SenderId");
CREATE INDEX IF NOT EXISTS idx_contacts_users ON "Contacts"("UserIdContactA", "UserIdContactB");
CREATE INDEX IF NOT EXISTS idx_users_phone ON "Users"("PhoneNumber");

-- ====================================
-- QUERIES FOR DISPLAYING USER INFO
-- (Similar to Messenger, WhatsApp, etc.)
-- ====================================

-- 1. Get user profile info to display (for user profile/contact card)
-- Shows: Phone, Full Name, Avatar, Online Status, Last Seen
SELECT 
    "Id",
    "PhoneNumber",
    "FirstName",
    "LastName",
    "AvatarUrl",
    "IsOnline",
    "LastSeen"
FROM "Users"
WHERE "Id" = :UserId;

-- 2. Get all users list (for searching, adding friends)
SELECT 
    "Id",
    "PhoneNumber",
    "FirstName",
    "LastName",
    "AvatarUrl",
    "IsOnline",
    "LastSeen"
FROM "Users"
ORDER BY "IsOnline" DESC, "LastSeen" DESC;

-- 3. Get user info in chat conversation (show sender's info at message)
SELECT 
    u."Id",
    u."PhoneNumber",
    u."FirstName",
    u."LastName",
    u."AvatarUrl",
    u."IsOnline"
FROM "Users" u
WHERE u."Id" = :UserId;

-- 4. Get chat partner info (when opening chat with someone)
SELECT 
    u."Id",
    u."PhoneNumber",
    u."FirstName",
    u."LastName",
    u."AvatarUrl",
    u."IsOnline",
    u."LastSeen",
    COUNT(m."Id") as "TotalMessages"
FROM "Users" u
LEFT JOIN "Messages" m ON m."SenderId" = u."Id"
WHERE u."Id" = :UserId
GROUP BY u."Id", u."PhoneNumber", u."FirstName", u."LastName", u."AvatarUrl", u."IsOnline", u."LastSeen";

-- 5. Search users by name or phone (for adding friends)
SELECT 
    "Id",
    "PhoneNumber",
    "FirstName",
    "LastName",
    "AvatarUrl",
    "IsOnline"
FROM "Users"
WHERE "FirstName" ILIKE :SearchTerm 
   OR "LastName" ILIKE :SearchTerm 
   OR "PhoneNumber" LIKE :SearchTerm
ORDER BY "FirstName" ASC;
