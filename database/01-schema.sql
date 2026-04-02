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
    "IsArchived" BOOLEAN DEFAULT false,
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
    "Status" VARCHAR(50) DEFAULT 'sent' CHECK ("Status" IN ('sent', 'delivered', 'read')),
    "IsDeleted" BOOLEAN DEFAULT false,
    "DeletedAt" TIMESTAMP,
    "EditedAt" TIMESTAMP,
    "SentAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Create Contacts (Friends) table
CREATE TABLE IF NOT EXISTS "Contacts" (
    "Id" SERIAL PRIMARY KEY,
    "UserIdContactA" INTEGER NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "UserIdContactB" INTEGER NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    CONSTRAINT unique_contact UNIQUE ("UserIdContactA", "UserIdContactB")
);

-- Create Read Receipts table (tracking who read which message)
CREATE TABLE IF NOT EXISTS "ReadReceipts" (
    "Id" SERIAL PRIMARY KEY,
    "MessageId" INTEGER NOT NULL REFERENCES "Messages"("Id") ON DELETE CASCADE,
    "UserId" INTEGER NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "ReadAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT unique_read_receipt UNIQUE ("MessageId", "UserId")
);

-- Create Blocked Users table (blocking feature)
CREATE TABLE IF NOT EXISTS "BlockedUsers" (
    "Id" SERIAL PRIMARY KEY,
    "BlockerId" INTEGER NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "BlockedUserId" INTEGER NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "BlockedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT unique_blocked UNIQUE ("BlockerId", "BlockedUserId"),
    CONSTRAINT no_self_block CHECK ("BlockerId" != "BlockedUserId")
);

-- Create indexes for better query performance
CREATE INDEX IF NOT EXISTS idx_chats_users ON "Chats"("UserAId", "UserBId");
CREATE INDEX IF NOT EXISTS idx_messages_chat ON "Messages"("ChatId");
CREATE INDEX IF NOT EXISTS idx_messages_sender ON "Messages"("SenderId");
CREATE INDEX IF NOT EXISTS idx_messages_is_deleted ON "Messages"("IsDeleted");
CREATE INDEX IF NOT EXISTS idx_contacts_users ON "Contacts"("UserIdContactA", "UserIdContactB");
CREATE INDEX IF NOT EXISTS idx_users_phone ON "Users"("PhoneNumber");
CREATE INDEX IF NOT EXISTS idx_read_receipts_message ON "ReadReceipts"("MessageId");
CREATE INDEX IF NOT EXISTS idx_read_receipts_user ON "ReadReceipts"("UserId");
CREATE INDEX IF NOT EXISTS idx_blocked_users ON "BlockedUsers"("BlockerId", "BlockedUserId");
