-- 1. Bảng Destination
CREATE TABLE Destination (
    DestinationID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    FareAmount DECIMAL(18, 2) NOT NULL CHECK (FareAmount >= 0)
);

-- 2. Bảng TicketVendorMachine
CREATE TABLE TicketVendorMachine (
    MachineID INT IDENTITY(1,1) PRIMARY KEY,
    Location NVARCHAR(255) NOT NULL,
    Status VARCHAR(50) DEFAULT 'Active' 
        CHECK (Status IN ('Active', 'Maintenance', 'Offline'))
);

-- 3. Bảng Ticket
CREATE TABLE Ticket (
    TicketID INT IDENTITY(1,1) PRIMARY KEY,
    IssueDate DATETIME DEFAULT GETDATE(),
    ValidUntil DATETIME NOT NULL,
    BarcodeData VARCHAR(MAX) NOT NULL,
    DestinationID INT FOREIGN KEY REFERENCES Destination(DestinationID),
    MachineID INT FOREIGN KEY REFERENCES TicketVendorMachine(MachineID)
);

-- Tạo Index tăng tốc truy vấn cho vé
CREATE NONCLUSTERED INDEX IX_Ticket_MachineID ON Ticket(MachineID);
CREATE NONCLUSTERED INDEX IX_Ticket_IssueDate ON Ticket(IssueDate);

-- 4. Bảng Payment
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

-- Tạo Index bảo vệ tính duy nhất (1 vé chỉ có 1 giao dịch thanh toán thành công)
CREATE UNIQUE NONCLUSTERED INDEX IX_Payment_TicketID ON Payment(TicketID);