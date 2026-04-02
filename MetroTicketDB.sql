CREATE DATABASE MetroTicketDB;
GO

USE MetroTicketDB;
GO

-- ============================================
-- 1. Bảng Destination (Các ga đến từ Bến Thành)
-- ============================================
CREATE TABLE Destination (
    DestinationID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    FareAmount DECIMAL(18, 2) NOT NULL CHECK (FareAmount >= 0)
);

-- ============================================
-- 2. Bảng TicketVendorMachine (Các máy tại ga Bến Thành)
-- ============================================
CREATE TABLE TicketVendorMachine (
    MachineID INT IDENTITY(1,1) PRIMARY KEY,
    Location NVARCHAR(255) NOT NULL,
    Status VARCHAR(50) DEFAULT 'Active' 
        CHECK (Status IN ('Active', 'Maintenance', 'Offline'))
);

-- ============================================
-- 3. Bảng Ticket
-- ============================================
CREATE TABLE Ticket (
    TicketID INT IDENTITY(1,1) PRIMARY KEY,
    IssueDate DATETIME DEFAULT GETDATE(),
    ValidUntil DATETIME NOT NULL,
    BarcodeData VARCHAR(MAX) NOT NULL,
    DestinationID INT FOREIGN KEY REFERENCES Destination(DestinationID),
    MachineID INT FOREIGN KEY REFERENCES TicketVendorMachine(MachineID)
);

CREATE NONCLUSTERED INDEX IX_Ticket_MachineID ON Ticket(MachineID);
CREATE NONCLUSTERED INDEX IX_Ticket_IssueDate ON Ticket(IssueDate);

-- ============================================
-- 4. Bảng Payment
-- ============================================
CREATE TABLE Payment (
    PaymentID INT IDENTITY(1,1) PRIMARY KEY,
    TicketID INT FOREIGN KEY REFERENCES Ticket(TicketID),
    Amount DECIMAL(18, 2) NOT NULL CHECK (Amount >= 0),
    PaymentDate DATETIME DEFAULT GETDATE(),
    PaymentMethod VARCHAR(50) NOT NULL 
        CHECK (PaymentMethod IN ('CreditCard', 'Momo', 'VNPay', 'ZaloPay')),
    TransactionStatus VARCHAR(50) DEFAULT 'Completed'
        CHECK (TransactionStatus IN ('Pending', 'Completed', 'Failed', 'Refunded')),
    ProviderRef VARCHAR(100)
);

CREATE UNIQUE NONCLUSTERED INDEX IX_Payment_TicketID ON Payment(TicketID);

-- ============================================
-- NHẬP DỮ LIỆU GỐC (MASTER DATA) 
-- ============================================

-- 1. Thêm các máy bán vé đặt tại ga Bến Thành
INSERT INTO TicketVendorMachine (Location, Status) VALUES 
(N'Ben Thanh Station - Gate A', 'Active'),
(N'Ben Thanh Station - Gate B', 'Active'),
(N'Ben Thanh Station - Center Mall', 'Active');

-- 2. Thêm 8 ga đến xuất phát từ Bến Thành
INSERT INTO Destination (Name, FareAmount) VALUES 
(N'Ben Thanh -> Opera House', 15000.00),
(N'Ben Thanh -> Ba Son', 15000.00),
(N'Ben Thanh -> Van Thanh Park', 17000.00),
(N'Ben Thanh -> Thao Dien', 19000.00),
(N'Ben Thanh -> An Phu', 19000.00),
(N'Ben Thanh -> Rach Chiec', 21000.00),
(N'Ben Thanh -> Hi-Tech Park', 23000.00),
(N'Ben Thanh -> Suoi Tien', 25000.00);