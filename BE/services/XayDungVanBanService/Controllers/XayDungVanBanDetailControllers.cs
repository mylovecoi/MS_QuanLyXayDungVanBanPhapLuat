using BuildingBlocks.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace XayDungVanBanService.Controllers;

[ApiController, Route("api/xay-dung-van-ban/danh-sach")]
public sealed class XayDungVanBanDanhSachController(ICurrentUserContext user) : XayDungVanBanControllerBase(user)
{ [HttpGet] public ActionResult Get() => Index("VanBanQPPL.XayDungVanBan.DanhSach"); }

[ApiController, Route("api/xay-dung-van-ban/soan-thao")]
public sealed class XayDungVanBanSoanThaoController(ICurrentUserContext user) : XayDungVanBanControllerBase(user)
{ [HttpGet] public ActionResult Get() => Index("VanBanQPPL.XayDungVanBan.SoanThao"); }

[ApiController, Route("api/xay-dung-van-ban/trinh-tham-dinh")]
public sealed class XayDungVanBanTrinhThamDinhController(ICurrentUserContext user) : XayDungVanBanControllerBase(user)
{ [HttpGet] public ActionResult Get() => Index("VanBanQPPL.XayDungVanBan.TrinhThamDinh"); }

[ApiController, Route("api/xay-dung-van-ban/tham-dinh")]
public sealed class XayDungVanBanThamDinhController(ICurrentUserContext user) : XayDungVanBanControllerBase(user)
{ [HttpGet] public ActionResult Get() => Index("VanBanQPPL.XayDungVanBan.ThamDinh"); }

[ApiController, Route("api/xay-dung-van-ban/trinh-phe-duyet")]
public sealed class XayDungVanBanTrinhPheDuyetController(ICurrentUserContext user) : XayDungVanBanControllerBase(user)
{ [HttpGet] public ActionResult Get() => Index("VanBanQPPL.XayDungVanBan.TrinhPheDuyet"); }

[ApiController, Route("api/xay-dung-van-ban/y-kien-ubnd")]
public sealed class XayDungVanBanYKienUbndController(ICurrentUserContext user) : XayDungVanBanControllerBase(user)
{ [HttpGet] public ActionResult Get() => Index("VanBanQPPL.XayDungVanBan.YKienUbnd"); }

[ApiController, Route("api/xay-dung-van-ban/tham-tra-hdnd")]
public sealed class XayDungVanBanThamTraHdndController(ICurrentUserContext user) : XayDungVanBanControllerBase(user)
{ [HttpGet] public ActionResult Get() => Index("VanBanQPPL.XayDungVanBan.ThamTraHdnd"); }

[ApiController, Route("api/xay-dung-van-ban/ban-hanh")]
public sealed class XayDungVanBanBanHanhController(ICurrentUserContext user) : XayDungVanBanControllerBase(user)
{ [HttpGet] public ActionResult Get() => Index("VanBanQPPL.XayDungVanBan.BanHanh"); }
