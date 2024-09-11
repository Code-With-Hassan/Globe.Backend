SET IDENTITY_INSERT [Applications] ON;
    INSERT [Applications] ([Id], [Name], [IsActive], [CreatedAt], [UpdatedAt]) 
    VALUES (1, N'foundation', 1, 0, 0);
SET IDENTITY_INSERT [Applications] OFF;

SET IDENTITY_INSERT [Screens] ON;
    INSERT [Screens] ([Id], [ScreenName], [IsActive], [CreatedAt], [UpdatedAt], [ApplicationId], [ScreenPrivilege]) 
    VALUES (1, N'users', 1, 0, 0, 1, 1),
           (2, N'role', 1, 0, 0, 1, 1),
           (3, N'organizations', 1, 0, 0, 1, 1),
           (4, N'audit', 1, 0, 0, 1, 1);
SET IDENTITY_INSERT [Screens] OFF;

SET IDENTITY_INSERT [CommonAddresses] ON;
    INSERT [CommonAddresses] ([Id], [StreetName], [StreetNumber], [PostCode], [CityId], [IsActive], [CreatedAt], [UpdatedAt]) 
    VALUES (1, N'Test', N'12', N'18876', 1, 1, 0, 0);
SET IDENTITY_INSERT [CommonAddresses] OFF;

SET IDENTITY_INSERT [Organizations] ON;
    INSERT [Organizations] ([Id], [Name], [AddressId], [Phone], [Email], [IsActive], [CreatedAt], [UpdatedAt]) 
    VALUES (1, N'Globe', 1, N'2109987500', N'globe@mailinator.com', 1, 0, 0);
SET IDENTITY_INSERT [Organizations] OFF;

SET IDENTITY_INSERT [Users] ON;
    INSERT [Users] ([Id], [UserName], [IsSuperUser], [IsLocked], [IsActive], [CreatedAt], [UpdatedAt], [Email]) 
    VALUES (1, N'admin', 1, 0, 1, 0, 0, N'globe@mailinator.com');
SET IDENTITY_INSERT [Users] OFF;


INSERT [AspNetUsers] ([Id], [UserId], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp],
                            [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) 
VALUES (N'738bca4c-78c8-4d90-9308-cacff9b48490', 1, N'admin', N'ADMIN', N'globe@mailinator.com', N'GLOBE@MAILINATOR.COM', 1, 
        N'AQAAAAEAACcQAAAAENhD7/MWLbVJp2jKyCd/4nz9xJAq0V0w4L5zRVN4KIOt8N9F+fa6UqbGL/gu+7Dvow==', N'Z4NCVHASAKZ3YQX2C4TADF2TCX6DV4BI', 
        N'5d4a900f-680b-49e0-bf41-b26a674c649c', NULL, 0, 0, NULL, 1, 0);


SET IDENTITY_INSERT [Roles] ON;
    INSERT [Roles] ([Id], [RoleDescription], [DefaultApplicationId], [IsActive], [CreatedAt], [UpdatedAt]) 
    VALUES (1, N'Super Admin', 1, 1, 0, 0);
SET IDENTITY_INSERT [Roles] OFF;

    INSERT INTO [RoleOrganizations] ([RoleId] ,[OrganizationId], [IsActive], [CreatedAt], [UpdatedAt])
    VALUES (1 ,1 ,1 ,0 ,0)

SET IDENTITY_INSERT [UserRoles] ON;
    INSERT [UserRoles] ([UserId], [RoleId], [Id], [IsActive], [CreatedAt], [UpdatedAt]) 
    VALUES (1, 1, 1, 1, 0, 0);
SET IDENTITY_INSERT [UserRoles] OFF;


SET IDENTITY_INSERT [RoleScreens] ON;
    INSERT [RoleScreens] ([Id], [RoleId], [ScreenId], [Privilege], [IsActive], [CreatedAt], [UpdatedAt]) 
    VALUES (1, 1, 1, 1, 1, 0, 0),
           (2, 1, 2, 1, 1, 0, 0),
           (3, 1, 3, 1, 1, 0, 0),
           (4, 1, 4, 1, 1, 0, 0);
SET IDENTITY_INSERT [RoleScreens] OFF;