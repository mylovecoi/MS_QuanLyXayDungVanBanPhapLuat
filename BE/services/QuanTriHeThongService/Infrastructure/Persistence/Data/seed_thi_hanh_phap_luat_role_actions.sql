/*
    Seed RoleAction cho ThiHanhPhapLuatService.
    Các chức năng được treo trực tiếp dưới group ThiHanhPhapLuat hiện hữu trong dữ liệu import.
    Script idempotent: nếu seed cũ VanBanQPPL.ThiHanhPhapLuat đã chạy thì cập nhật lại theo Id.
*/
SET NOCOUNT ON;

DECLARE @Now datetime2 = SYSUTCDATETIME();
DECLARE @SystemUserId uniqueidentifier = '00000000-0000-0000-0000-000000000000';
DECLARE @ParentId uniqueidentifier;

SELECT @ParentId = [Id]
FROM [qtht].[RoleActions]
WHERE [Id] = '20000000-0000-0000-0000-000000000031'
   OR [Role] = N'ThiHanhPhapLuat';

IF @ParentId IS NULL
    THROW 51000, N'Không tìm thấy RoleAction cha ThiHanhPhapLuat.', 1;

DECLARE @Rows TABLE
(
    [Id] uniqueidentifier NOT NULL,
    [STTSapXep] int NOT NULL,
    [Role] nvarchar(250) NOT NULL,
    [Title] nvarchar(500) NOT NULL,
    [Controller] nvarchar(250) NOT NULL,
    [Table] nvarchar(250) NOT NULL,
    [FrontendPath] nvarchar(500) NOT NULL,
    [MenuTitle] nvarchar(500) NOT NULL,
    [MenuIcon] nvarchar(100) NOT NULL
);

INSERT INTO @Rows VALUES
('93000000-0000-0000-0000-000000000002', 301, N'ThiHanhPhapLuat.DanhSach', N'Danh sách thi hành pháp luật', N'ThiHanhPhapLuatDanhSach', N'KeHoachThiHanhPhapLuats', N'/thi-hanh-phap-luat/danh-sach', N'Danh sách', N'list'),
('93000000-0000-0000-0000-000000000003', 302, N'ThiHanhPhapLuat.KeHoach', N'Quản lý kế hoạch thi hành pháp luật', N'ThiHanhPhapLuatKeHoach', N'KeHoachThiHanhPhapLuats', N'/thi-hanh-phap-luat/ke-hoach', N'Kế hoạch', N'file-text'),
('93000000-0000-0000-0000-000000000004', 303, N'ThiHanhPhapLuat.NoiDungKeHoach', N'Nội dung kế hoạch thi hành pháp luật', N'ThiHanhPhapLuatNoiDungKeHoach', N'NoiDungKeHoachs', N'/thi-hanh-phap-luat/noi-dung-ke-hoach', N'Nội dung kế hoạch', N'list-todo'),
('93000000-0000-0000-0000-000000000005', 304, N'ThiHanhPhapLuat.PhanCong', N'Phân công thực hiện kế hoạch', N'ThiHanhPhapLuatPhanCong', N'PhanCongThiHanhs', N'/thi-hanh-phap-luat/phan-cong', N'Phân công', N'users'),
('93000000-0000-0000-0000-000000000006', 305, N'ThiHanhPhapLuat.TienDo', N'Cập nhật tình hình thực hiện', N'ThiHanhPhapLuatTienDo', N'BaoCaoTienDoThiHanhs', N'/thi-hanh-phap-luat/tien-do', N'Tình hình thực hiện', N'chart-line'),
('93000000-0000-0000-0000-000000000007', 306, N'ThiHanhPhapLuat.DanhGia', N'Đánh giá kết quả thi hành pháp luật', N'ThiHanhPhapLuatDanhGia', N'DanhGiaThiHanhs', N'/thi-hanh-phap-luat/danh-gia', N'Đánh giá kết quả', N'clipboard-check'),
('93000000-0000-0000-0000-000000000008', 307, N'ThiHanhPhapLuat.TongHop', N'Tổng hợp kết quả thi hành pháp luật', N'ThiHanhPhapLuatTongHop', N'BaoCaoTongHopThiHanhs', N'/thi-hanh-phap-luat/tong-hop', N'Tổng hợp kết quả', N'file-chart-column');

INSERT INTO [qtht].[RoleActions]
(
    [Id], [STTSapXep], [PhanLoai], [Level], [Role], [RoleGroupId], [Title],
    [Controller], [Action], [Parameter], [Table], [Status], [UseGroup],
    [FrontendPath], [IsVisibleInMenu], [ClientApp], [MenuTitle], [MenuIcon], [Icon],
    [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]
)
SELECT r.[Id], r.[STTSapXep], N'Detail', 1, r.[Role], @ParentId, r.[Title],
       r.[Controller], N'Index', NULL, r.[Table], N'Kích hoạt', NULL,
       r.[FrontendPath], 1, N'frontend', r.[MenuTitle], r.[MenuIcon], r.[MenuIcon],
       @SystemUserId, @Now, @SystemUserId, @Now
FROM @Rows r
WHERE NOT EXISTS (SELECT 1 FROM [qtht].[RoleActions] e WHERE e.[Id] = r.[Id] OR e.[Role] = r.[Role]);

UPDATE e
SET e.[STTSapXep] = r.[STTSapXep],
    e.[PhanLoai] = N'Detail',
    e.[Level] = 1,
    e.[Role] = r.[Role],
    e.[RoleGroupId] = @ParentId,
    e.[Title] = r.[Title],
    e.[Controller] = r.[Controller],
    e.[Action] = N'Index',
    e.[Table] = r.[Table],
    e.[Status] = N'Kích hoạt',
    e.[FrontendPath] = r.[FrontendPath],
    e.[IsVisibleInMenu] = 1,
    e.[ClientApp] = N'frontend',
    e.[MenuTitle] = r.[MenuTitle],
    e.[MenuIcon] = r.[MenuIcon],
    e.[Icon] = r.[MenuIcon],
    e.[UpdatedBy] = @SystemUserId,
    e.[UpdatedDate] = @Now
FROM [qtht].[RoleActions] e
JOIN @Rows r ON r.[Id] = e.[Id] OR r.[Role] = e.[Role];

SELECT [Role], [PhanLoai], [Controller], [Action], [FrontendPath], [Table], [Status], [IsVisibleInMenu]
FROM [qtht].[RoleActions]
WHERE [Role] LIKE N'ThiHanhPhapLuat.%'
ORDER BY [STTSapXep];
