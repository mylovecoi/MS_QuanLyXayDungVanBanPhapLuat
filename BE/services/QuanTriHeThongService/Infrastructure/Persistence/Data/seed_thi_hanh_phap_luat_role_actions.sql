/* Seed RoleAction: mỗi Detail có đúng một form và controller. */
SET NOCOUNT ON;
DECLARE @Now datetime2 = SYSUTCDATETIME();
DECLARE @SystemUserId uniqueidentifier = '00000000-0000-0000-0000-000000000000';
DECLARE @ParentId uniqueidentifier;
DECLARE @GroupId uniqueidentifier = '93000000-0000-0000-0000-000000000001';
SELECT @ParentId = [Id] FROM [qtht].[RoleActions] WHERE [Role] = N'VanBanQPPL';
IF @ParentId IS NULL THROW 51000, N'Không tìm thấy RoleAction cha VanBanQPPL.', 1;

DECLARE @Rows TABLE (Id uniqueidentifier, Stt int, PhanLoai nvarchar(50), Level int, Role nvarchar(250), Title nvarchar(500), Controller nvarchar(250), [Table] nvarchar(250), Path nvarchar(500), Menu nvarchar(500), Icon nvarchar(100));
INSERT INTO @Rows VALUES
(@GroupId, 300, N'Group', 0, N'VanBanQPPL.ThiHanhPhapLuat', N'Thi hành pháp luật', NULL, NULL, N'/thi-hanh-phap-luat', N'Thi hành pháp luật', N'clipboard-list'),
('93000000-0000-0000-0000-000000000002', 301, N'Detail', 1, N'VanBanQPPL.ThiHanhPhapLuat.DanhSach', N'Danh sách thi hành pháp luật', N'ThiHanhPhapLuatDanhSach', N'KeHoachThiHanhPhapLuats', N'/thi-hanh-phap-luat/danh-sach', N'Danh sách', N'list'),
('93000000-0000-0000-0000-000000000003', 302, N'Detail', 1, N'VanBanQPPL.ThiHanhPhapLuat.KeHoach', N'Kế hoạch thi hành pháp luật', N'ThiHanhPhapLuatKeHoach', N'KeHoachThiHanhPhapLuats', N'/thi-hanh-phap-luat/ke-hoach', N'Kế hoạch', N'file-text'),
('93000000-0000-0000-0000-000000000004', 303, N'Detail', 1, N'VanBanQPPL.ThiHanhPhapLuat.NoiDungKeHoach', N'Nội dung kế hoạch', N'ThiHanhPhapLuatNoiDungKeHoach', N'NoiDungKeHoachs', N'/thi-hanh-phap-luat/noi-dung-ke-hoach', N'Nội dung kế hoạch', N'list-todo'),
('93000000-0000-0000-0000-000000000005', 304, N'Detail', 1, N'VanBanQPPL.ThiHanhPhapLuat.PhanCong', N'Phân công thi hành', N'ThiHanhPhapLuatPhanCong', N'PhanCongThiHanhs', N'/thi-hanh-phap-luat/phan-cong', N'Phân công', N'users'),
('93000000-0000-0000-0000-000000000006', 305, N'Detail', 1, N'VanBanQPPL.ThiHanhPhapLuat.TienDo', N'Tiến độ thi hành', N'ThiHanhPhapLuatTienDo', N'BaoCaoTienDoThiHanhs', N'/thi-hanh-phap-luat/tien-do', N'Tiến độ', N'chart-line'),
('93000000-0000-0000-0000-000000000007', 306, N'Detail', 1, N'VanBanQPPL.ThiHanhPhapLuat.DanhGia', N'Đánh giá thi hành', N'ThiHanhPhapLuatDanhGia', N'DanhGiaThiHanhs', N'/thi-hanh-phap-luat/danh-gia', N'Đánh giá', N'clipboard-check'),
('93000000-0000-0000-0000-000000000008', 307, N'Detail', 1, N'VanBanQPPL.ThiHanhPhapLuat.TongHop', N'Tổng hợp thi hành', N'ThiHanhPhapLuatTongHop', N'BaoCaoTongHopThiHanhs', N'/thi-hanh-phap-luat/tong-hop', N'Tổng hợp', N'file-chart-column'),
('93000000-0000-0000-0000-000000000009', 308, N'Detail', 1, N'VanBanQPPL.ThiHanhPhapLuat.Dashboard', N'Dashboard thi hành pháp luật', N'ThiHanhPhapLuatDashboard', N'KeHoachThiHanhPhapLuats', N'/thi-hanh-phap-luat/dashboard', N'Dashboard', N'layout-dashboard');

INSERT INTO [qtht].[RoleActions] ([Id], [STTSapXep], [PhanLoai], [Level], [Role], [RoleGroupId], [Title], [Controller], [Action], [Table], [Status], [FrontendPath], [IsVisibleInMenu], [ClientApp], [MenuTitle], [MenuIcon], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate])
SELECT r.Id, r.Stt, r.PhanLoai, r.Level, r.Role, CASE WHEN r.Role = N'VanBanQPPL.ThiHanhPhapLuat' THEN @ParentId ELSE @GroupId END, r.Title, r.Controller, CASE WHEN r.PhanLoai = N'Detail' THEN N'Index' END, r.[Table], N'Kích hoạt', r.Path, 1, N'frontend', r.Menu, r.Icon, @SystemUserId, @Now, @SystemUserId, @Now
FROM @Rows r WHERE NOT EXISTS (SELECT 1 FROM [qtht].[RoleActions] e WHERE e.Role = r.Role);

UPDATE e SET e.[STTSapXep] = r.Stt, e.[PhanLoai] = r.PhanLoai, e.[Level] = r.Level, e.[RoleGroupId] = CASE WHEN r.Role = N'VanBanQPPL.ThiHanhPhapLuat' THEN @ParentId ELSE @GroupId END, e.[Title] = r.Title, e.[Controller] = r.Controller, e.[Action] = CASE WHEN r.PhanLoai = N'Detail' THEN N'Index' ELSE NULL END, e.[Table] = r.[Table], e.[Status] = N'Kích hoạt', e.[FrontendPath] = r.Path, e.[IsVisibleInMenu] = 1, e.[ClientApp] = N'frontend', e.[MenuTitle] = r.Menu, e.[MenuIcon] = r.Icon, e.[UpdatedBy] = @SystemUserId, e.[UpdatedDate] = @Now
FROM [qtht].[RoleActions] e JOIN @Rows r ON r.Role = e.Role;
