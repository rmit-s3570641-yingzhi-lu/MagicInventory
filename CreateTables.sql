drop table StockRequest
drop table StoreInventory
drop table OwnerInventory
drop table Store
drop table Product

create table Product
(
	ProductID int not null identity,
	Name nvarchar(25) not null,
    constraint PK_Product primary key (ProductID)
);

create table Store
(
	StoreID int not null identity,
	Name nvarchar(25) not null,
    constraint PK_Store primary key (StoreID)
);

create table OwnerInventory
(
	ProductID int not null,
	StockLevel int not null,
    constraint PK_OwnerInventory primary key (ProductID),
    constraint FK_OwnerInventory_Product foreign key
    (ProductID) references Product (ProductID)
);

create table StoreInventory
(
    StoreID int not null,
    ProductID int not null,
    StockLevel int not null,
    constraint PK_StoreInventory primary key (StoreID, ProductID),
    constraint FK_StoreInventory_Store foreign key
    (StoreID) references Store (StoreID),
    constraint FK_StoreInventory_Product foreign key
    (ProductID) references Product (ProductID)
);

create table StockRequest
(
    StockRequestID int not null identity,
    StoreID int not null,
    ProductID int not null,
    Quantity int not null,
    constraint PK_StockRequest primary key (StockRequestID),
    constraint FK_StockRequest_Store foreign key
    (StoreID) references Store (StoreID),
    constraint FK_StockRequest_Product foreign key
    (ProductID) references Product (ProductID)
);


declare @rabbit int;
insert into Product (Name) values ('Rabbit');
set @rabbit = scope_identity();

declare @hat int;
insert into Product (Name) values ('Hat');
set @hat = scope_identity();

declare @svengaliDeck int;
insert into Product (Name) values ('Svengali Deck');
set @svengaliDeck = scope_identity();

declare @floatingHankerchief int;
insert into Product (Name) values ('Floating Hankerchief');
set @floatingHankerchief = scope_identity();

declare @wand int;
insert into Product (Name) values ('Wand');
set @wand = scope_identity();

declare @broomstick int;
insert into Product (Name) values ('Broomstick');
set @broomstick = scope_identity();

declare @bangGun int;
insert into Product (Name) values ('Bang Gun');
set @bangGun = scope_identity();

declare @cloakOfInvisibility int;
insert into Product (Name) values ('Cloak of Invisibility');
set @cloakOfInvisibility = scope_identity();

declare @elderWand int;
insert into Product (Name) values ('Elder Wand');
set @elderWand = scope_identity();

declare @resurrectionStone int;
insert into Product (Name) values ('Resurrection Stone');
set @resurrectionStone = scope_identity();

--

insert into OwnerInventory (ProductID, StockLevel) values (@rabbit, 20);
insert into OwnerInventory (ProductID, StockLevel) values (@hat, 50);
insert into OwnerInventory (ProductID, StockLevel) values (@svengaliDeck, 100);
insert into OwnerInventory (ProductID, StockLevel) values (@floatingHankerchief, 150);
insert into OwnerInventory (ProductID, StockLevel) values (@wand, 40);
insert into OwnerInventory (ProductID, StockLevel) values (@broomstick, 10);
insert into OwnerInventory (ProductID, StockLevel) values (@bangGun, 5);
insert into OwnerInventory (ProductID, StockLevel) values (@cloakOfInvisibility, 0);
insert into OwnerInventory (ProductID, StockLevel) values (@elderWand, 0);
insert into OwnerInventory (ProductID, StockLevel) values (@resurrectionStone, 0);

--

declare @cbd int;
insert into Store (Name) values ('Melbourne CBD');
set @cbd = scope_identity();

declare @north int;
insert into Store (Name) values ('North Melbourne');
set @north = scope_identity();

declare @east int;
insert into Store (Name) values ('East Melbourne');
set @east = scope_identity();

declare @south int;
insert into Store (Name) values ('South Melbourne');
set @south = scope_identity();

declare @west int;
insert into Store (Name) values ('West Melbourne');
set @west = scope_identity();

--

insert into StoreInventory (StoreID, ProductID, StockLevel) values (@cbd, @rabbit, 15);
insert into StoreInventory (StoreID, ProductID, StockLevel) values (@cbd, @hat, 10);
insert into StoreInventory (StoreID, ProductID, StockLevel) values (@cbd, @svengaliDeck, 5);
insert into StoreInventory (StoreID, ProductID, StockLevel) values (@cbd, @floatingHankerchief, 5);
insert into StoreInventory (StoreID, ProductID, StockLevel) values (@cbd, @wand, 5);
insert into StoreInventory (StoreID, ProductID, StockLevel) values (@cbd, @broomstick, 5);
insert into StoreInventory (StoreID, ProductID, StockLevel) values (@cbd, @bangGun, 5);
insert into StoreInventory (StoreID, ProductID, StockLevel) values (@cbd, @cloakOfInvisibility, 1);
insert into StoreInventory (StoreID, ProductID, StockLevel) values (@cbd, @elderWand, 1);
insert into StoreInventory (StoreID, ProductID, StockLevel) values (@cbd, @resurrectionStone, 1);

--

insert into StoreInventory (StoreID, ProductID, StockLevel) values (@north, @rabbit, 5);

--

insert into StoreInventory (StoreID, ProductID, StockLevel) values (@east, @hat, 5);

--

insert into StoreInventory (StoreID, ProductID, StockLevel) values (@south, @svengaliDeck, 5);

  SET IDENTITY_INSERT [s3570641].[dbo].[StockRequest] ON
  Insert into StockRequest (StockRequestID,StoreID,ProductID,Quantity) values(1,1,5,100)
  Insert into StockRequest (StockRequestID,StoreID,ProductID,Quantity) values(2,1,7,10)
