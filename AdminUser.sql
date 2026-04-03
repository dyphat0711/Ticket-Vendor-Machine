use MetroTicketDB;

CREATE TABLE tblUser (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL,
    Password NVARCHAR(50) NOT NULL,
    Role NVARCHAR(20) DEFAULT 'Admin'
);

-- Tạo một tài khoản mặc định để test
INSERT INTO tblUser (Username, Password, Role) 
VALUES ('admin', '123456', 'Admin');