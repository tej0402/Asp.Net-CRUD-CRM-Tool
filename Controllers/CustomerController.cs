using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Customer.Models;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Configuration;

namespace Customer.Controllers
{
    public class CustomerController : Controller
    {

        private object cust;
        private IEnumerable<FollowUpModel> cus;

        public string Lastname { get; private set; }
        public string Pincode { get; private set; }
        public int CustomerId { get; private set; }
        public object CustomerFollowups { get; private set; }
        public bool Close { get; private set; }
        public int Status { get; private set; }
        //public string FileName { get; private set; }

        #region PARTIAL VIEW TO STRING
        [NonAction]
        public string RenderRazorViewToString(string viewName, object model)
        {
            string strReturn = string.Empty;
            try
            {
                ViewData.Model = model;
                using (var sw = new StringWriter())
                {
                    var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                    var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                    viewResult.View.Render(viewContext, sw);
                    viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                    strReturn = sw.GetStringBuilder().ToString();
                }
            }
            catch (Exception ex)
            {
                string errormsg = ex.Message;
                if (ex.InnerException != null) errormsg = errormsg + " - " + ex.InnerException.Message;
            }
            return strReturn;
        }
        #endregion

        #region CUSTOMER

        private customerdbEntities2 db = new customerdbEntities2();
        public const int PageSize = 10;
        private string filename;
        private object filePath;

        [HttpGet]
        public ActionResult Index(int? id)
        {
            CustomerModel model = new CustomerModel();
            try
            {
                List<CustomerModel> cusDetailsList = new List<CustomerModel>();
                customerdbEntities2 db = new customerdbEntities2();
                var cus = db.GetCustomerdb(null).ToList();
                foreach (var item in cus)
                {
                    int? Pin = null;
                    if (!string.IsNullOrEmpty(item.Pincode))
                    {
                        Pin = Convert.ToInt32(item.Pincode.Trim());
                    }
                    cusDetailsList.Add(new CustomerModel()
                    {
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Gender = item.Gender.Value,
                        DoB = item.DoB,
                        Email = item.Email,
                        PhoneNumber = item.PhoneNumber,
                        Address = item.Address,
                        City = item.City,
                        State = item.State,
                        Pincode = Pin,
                        CustomerId = item.CustomerId,
                        FollowUpCount = item.FollowUpCount
                    });
                }
                model.cusDetailsList = cusDetailsList;
                if (id != null)
                {
                    var cusDet = cusDetailsList.Where(m => m.CustomerId == id).FirstOrDefault();
                    model.FirstName = cusDet.FirstName;
                    model.LastName = cusDet.LastName;
                    model.Gender = cusDet.Gender;
                    model.DoB = cusDet.DoB;
                    model.Email = cusDet.Email;
                    model.PhoneNumber = cusDet.PhoneNumber;
                    model.Address = cusDet.Address;
                    model.City = cusDet.City;
                    model.State = cusDet.State;
                    model.Pincode = cusDet.Pincode;
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(CustomerModel model)
        {
            customerdbEntities2 db = new customerdbEntities2();
            string message = string.Empty;
            bool status = false;
            try
            {
                if (model.DoB.Value.Year <= 2000)
                {
                    if (!model.CustomerId.HasValue)
                    {
                        db.SaveCustomerdb(model.FirstName, model.LastName, model.Gender, model.DoB, model.Email, model.PhoneNumber, model.Address, model.City, model.State, model.Pincode.ToString(), null, null);
                        message = "Record saved successfully.";
                        status = true;
                    }
                    else
                    {
                        db.UpdateCustomerdb(model.CustomerId, model.FirstName, model.LastName, model.Gender, model.DoB, model.Email, model.PhoneNumber, model.Address, model.City, model.State, model.Pincode.ToString(), null, model.ImagePath);
                        message = "Record updated successfully.";
                        status = true;
                    }
                }
                else
                {
                    message = "Date error, Please try again.";
                    status = false;
                }
                List<CustomerModel> cusDetailsList = new List<CustomerModel>();
                var cus = db.GetCustomerdb(null).ToList();
                foreach (var item in cus)
                {
                    cusDetailsList.Add(new CustomerModel()
                    {
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Gender = item.Gender.Value,
                        DoB = item.DoB,
                        Email = item.Email,
                        PhoneNumber = item.PhoneNumber,
                        Address = item.Address,
                        City = item.City,
                        State = item.State,
                        Pincode = Convert.ToInt32(item.Pincode),
                        CustomerId = item.CustomerId
                    });
                }
                model.cusDetailsList = cusDetailsList;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                message = "ERROR! Please Try Again";
                status = false;
            }
            return Json(new
            {
                status = status,
                message = message,
                content = RenderRazorViewToString("_PartialCustomerDetailsList", model)
            }
            );
        }

        [HttpPost]
        public ActionResult DeleteCustomer(int id)
        {
            CustomerModel model = new CustomerModel();

            customerdbEntities2 db = new customerdbEntities2();
            
            string message = string.Empty;
            bool status = false;
            try
            {
                //var Cusdets = db.GetCustomerdb(id).FirstOrDefault();
                //var filepath = Request.MapPath("~/CustomerImages/" + Cusdets.ImagePath);

                //System.IO.File.SetAttributes(filepath, FileAttributes.Normal);
                //System.IO.File.Delete(filepath);


                //Delete All Files In Folder
                //System.IO.DirectoryInfo folderInfo = new DirectoryInfo(folderPath);
                //    foreach (FileInfo file in folderInfo.GetFiles())
                //    {
                //    file.Delete();
                //}
                //foreach (DirectoryInfo dir in folderInfo.GetDirectories())
                //{
                //    dir.Delete(true);

                var Cusdet = db.DeleteCustomerdb(id);
                var cus = db.GetCustomerdb(null).ToList();
                

                List<CustomerModel> cusDetailsList = new List<CustomerModel>();
                foreach (var item in cus)
                {
                    cusDetailsList.Add(new CustomerModel()
                    {
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Gender = item.Gender.Value,
                        DoB = item.DoB,
                        Email = item.Email,
                        PhoneNumber = item.PhoneNumber,
                        Address = item.Address,
                        City = item.City,
                        State = item.State,
                        Pincode = Convert.ToInt32(item.Pincode),
                        CustomerId = item.CustomerId
                    });
                }
                model.cusDetailsList = cusDetailsList;
                message = "Record deleted successfully.";
                status = true;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                message = "Error! This User Has Followup.";
                status = false;
            }
            return Json(new
            {
                status = status,
                message = message,
                content = RenderRazorViewToString("_PartialCustomerDetailsList", model)
            });
        }
        [HttpGet]
        public ActionResult EditCustomer(int? id)
        {
            customerdbEntities2 db = new customerdbEntities2();
            CustomerModel model = new CustomerModel();
            if (id != null)
            {
                var Cusdet = db.GetCustomerdb(id).FirstOrDefault();
                if (Cusdet != null)
                {
                    model.CustomerId = Cusdet.CustomerId;
                    model.FirstName = Cusdet.FirstName;
                    model.LastName = Cusdet.LastName;
                    model.Gender = Cusdet.Gender.Value;
                    model.DoB = Cusdet.DoB;
                    model.Email = Cusdet.Email;
                    model.PhoneNumber = Cusdet.PhoneNumber;
                    model.Address = Cusdet.Address;
                    model.City = Cusdet.City;
                    model.State = Cusdet.State;
                    model.Pincode = Convert.ToInt32(Cusdet.Pincode);
                }
            }
            return PartialView("_EditFormCustomer", model);
        }
        #endregion

        #region CUSTOMER FOLLOWUPS

        [HttpGet]
        public ActionResult FollowupMasterView(int id)//id=cust id
        {
            FollowUpModel model = new FollowUpModel();
            try
            {
                List<FollowUpModel> cusDetailsList = new List<FollowUpModel>();
                customerdbEntities2 db = new customerdbEntities2();
                var cus = db.CustomerFollowUpsGet(null, id).ToList();
                foreach (var item in cus)
                {
                    cusDetailsList.Add(new FollowUpModel()
                    {
                        FollowUpDate = item.FollowUpDate.Value,
                        FollowUpType = item.FollowUpType.Value,
                        Comments = item.Comments,
                        Status = item.Status.Value,
                        NextFollowUpDate = item.NextFollowUpDate.Value,
                        FollowUpId = item.FollowUpId,
                        Cid = item.CustomerId.Value
                    });
                }
                model.cusDetailsList = cusDetailsList;
                var Details = db.GetCustomerdb(id).FirstOrDefault();
                model.FirstName = Details.FirstName;

            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult FollowupMasterView(FollowUpModel model, int? id)//id=customerid
        {

            customerdbEntities2 db = new customerdbEntities2();
            string message = string.Empty;
            bool status = false;


            try
            {
                if (model.FollowUpDate.Value <= DateTime.Now)
                {
                    if (model.NextFollowUpDate > model.FollowUpDate)
                    {
                        if (!model.FollowUpId.HasValue)
                        {
                            db.CustomerFollowUpsInsert(model.FollowUpDate, model.FollowUpType, model.Comments, model.Status, model.NextFollowUpDate, null, id);
                            message = "Record saved successfully.";
                            status = true;
                        }
                        else
                        {
                            db.CustomerFollowUpsUpdate(model.FollowUpId, id, model.FollowUpDate, model.FollowUpType, model.Comments, model.Status, model.NextFollowUpDate, null);
                            message = "Record updated successfully.";
                            status = true;
                        }
                    }
                    else
                    {
                        message = "Next FollowUp Date Must Be Greater.";
                    }
                }

                else
                {
                    message = "Date error, Please try again.";
                    status = false;
                }

                List<FollowUpModel> cusDetailsList = new List<FollowUpModel>();


                var cus = db.CustomerFollowUpsGet(null, id).ToList();


                foreach (var item in cus)
                {

                    cusDetailsList.Add(new FollowUpModel()
                    {

                        FollowUpDate = item.FollowUpDate.Value,
                        FollowUpType = item.FollowUpType.Value,
                        Comments = item.Comments,
                        Status = item.Status.Value,
                        NextFollowUpDate = item.NextFollowUpDate,
                        FollowUpId = item.FollowUpId,
                        Cid = item.CustomerId.Value

                    });

                }



                model.cusDetailsList = cusDetailsList;
            }

            catch (Exception ex)
            {
                string error = ex.Message;
                message = "ERROR! Please Try Again";
                status = false;
            }
            return Json(new
            {
                status = status,
                message = message,
                content = RenderRazorViewToString("FollowUpCustomerDetailsList", model)
            }
             );
        }

        private void @if(object v)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public ActionResult EditCustomerFollowup(int id)//id=followupid
        {
            customerdbEntities2 db = new customerdbEntities2();

            FollowUpModel model = new FollowUpModel();
            if (id != null)
            {

                var Cusdet = db.CustomerFollowUpsGet(id, null).FirstOrDefault();
                if (Cusdet != null)
                {
                    model.FollowUpId = Cusdet.FollowUpId;
                    model.FollowUpDate = Cusdet.FollowUpDate.Value;
                    model.FollowUpType = Cusdet.FollowUpType.Value;
                    model.Comments = Cusdet.Comments;
                    model.Status = Cusdet.Status.Value;
                    model.NextFollowUpDate = Cusdet.NextFollowUpDate;
                    model.Cid = Cusdet.CustomerId.Value;
                }
            }
            return PartialView("EditCustomerFollowupForm", model);
        }

        [HttpPost]
        public ActionResult EditCustomerFollowup(FollowUpModel model)//id=customerid
        {

            customerdbEntities2 db = new customerdbEntities2();
            string message = string.Empty;
            bool status = false;
            FollowUpModel fc = new FollowUpModel();


            try
            {
                if (model.FollowUpDate.Value <= DateTime.Now)
                {
                    if (model.NextFollowUpDate > model.FollowUpDate)
                    {
                        if (!model.FollowUpId.HasValue)
                        {
                            //db.CustomerFollowUpsInsert(model.FollowUpDate, model.FollowUpType, model.Comments, model.Status, model.NextFollowUpDate, null, id);
                            //message = "Record saved successfully.";
                            //status = true;
                        }
                        else
                        {
                            db.CustomerFollowUpsUpdate(model.FollowUpId, model.Cid, model.FollowUpDate, model.FollowUpType, model.Comments, model.Status, model.NextFollowUpDate, null);
                            message = "Record updated successfully.";
                            status = true;
                        }
                    }
                    else
                    {
                        message = "Next FollowUp Date Must Be Greater.";
                    }
                }

                else
                {
                    message = "Date error, Please try again.";
                    status = false;
                }

                List<FollowUpModel> cusDetailsList = new List<FollowUpModel>();
                if (model.FollowUpId != null)
                {

                    var cus = db.CustomerFollowUpsGet(null, model.Cid).ToList();


                    foreach (var item in cus)
                    {

                        cusDetailsList.Add(new FollowUpModel()
                        {

                            FollowUpDate = item.FollowUpDate.Value,
                            FollowUpType = item.FollowUpType.Value,
                            Comments = item.Comments,
                            Status = item.Status.Value,
                            NextFollowUpDate = item.NextFollowUpDate,
                            FollowUpId = item.FollowUpId,
                            Cid = item.CustomerId.Value

                        });

                    }


                }
                model.cusDetailsList = cusDetailsList;
            }

            catch (Exception ex)
            {
                string error = ex.Message;
                message = "ERROR! Please Try Again";
                status = false;
            }

            return Json(new
            {
                status = status,
                message = message,
                content = RenderRazorViewToString("FollowUpCustomerDetailsList", model)
                //,
                //content1 = RenderRazorViewToString("FollowUpAddForm",fc)
            }
             );

            // ModelState.Clear();
        }

        private object RenderRazorViewToString(string v)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public ActionResult DeleteCustomerFollowup(int id, int cid)//id=followupid cid=custid
        {
            customerdbEntities2 db = new customerdbEntities2();
            FollowUpModel model = new FollowUpModel();
            string message = string.Empty;
            bool status = false;
            try
            {
                var Cusdet = db.CustomerFollowUpsDelete(id);
                var cus = db.CustomerFollowUpsGet(null, cid).ToList();
                List<FollowUpModel> cusDetailsList = new List<FollowUpModel>();
                foreach (var item in cus)
                {
                    cusDetailsList.Add(new FollowUpModel()
                    {
                        FollowUpDate = item.FollowUpDate.Value,
                        FollowUpType = item.FollowUpType.Value,
                        Comments = item.Comments,
                        Status = item.Status.Value,
                        NextFollowUpDate = item.NextFollowUpDate.Value,
                        FollowUpId = item.FollowUpId,
                        Cid = item.CustomerId.Value

                    });
                }
                model.cusDetailsList = cusDetailsList;
                message = "Record deleted successfully.";
                status = true;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                message = "Record not deleted, try again.";
                status = false;
            }
            return Json(new
            {
                status = status,
                message = message,
                content = RenderRazorViewToString("FollowUpCustomerDetailsList", model)
            });

            //public ActionResult DeletePopUp()
            //{
            //    return View();
            //}
        }

        [HttpPost]
        public ActionResult StatusCustomerFollowup(FollowUpModel model, int id)
        {
            customerdbEntities2 db = new customerdbEntities2();
            string message = string.Empty;
            string Statustext = string.Empty;
            bool status = false;
            if (model.Status > 0)
            {
                model.Status = 0;
                db.CustomerFollowUpsStatus(id, model.Status);
                message = "Status Opened.";
                Statustext = "Open";
                status = true;

            }
            else
            {
                model.Status = 1;
                message = "Status Closed.";
                Statustext = "Closed";
                status = true;
            }
            db.CustomerFollowUpsStatus(id, model.Status);
            return Json(new { status = status, id = id, message = message, Statustext = Statustext });
        }
        #endregion

        [HttpGet]
        public ActionResult DisplayCustomerDetailsList(int? id)

        {
            CustomerModel model = new CustomerModel();
            customerdbEntities2 db = new customerdbEntities2();
            var cus = db.GetCustomerdb(null).ToList();
            List<CustomerModel> cusDetailsList = new List<CustomerModel>();
            foreach (var item in cus)
            {
                int? Pin = null;
                if (!string.IsNullOrEmpty(item.Pincode))
                {
                    Pin = Convert.ToInt32(item.Pincode.Trim());
                }
                cusDetailsList.Add(new CustomerModel()
                {
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Gender = item.Gender.Value,
                    DoB = item.DoB,
                    Email = item.Email,
                    PhoneNumber = item.PhoneNumber,
                    Address = item.Address,
                    City = item.City,
                    State = item.State,
                    Pincode = Pin,
                    CustomerId = item.CustomerId,
                    FollowUpCount = item.FollowUpCount,
                    ImagePath = item.ImagePath
                });
            }
            model.cusDetailsList = cusDetailsList;
            if (id != null)
            {
                var cusDet = cusDetailsList.Where(m => m.CustomerId == id).FirstOrDefault();
                model.FirstName = cusDet.FirstName;
                model.LastName = cusDet.LastName;
                model.Gender = cusDet.Gender;
                model.DoB = cusDet.DoB;
                model.Email = cusDet.Email;
                model.PhoneNumber = cusDet.PhoneNumber;
                model.Address = cusDet.Address;
                model.City = cusDet.City;
                model.State = cusDet.State;
                model.Pincode = cusDet.Pincode;
            }

            return View("DisplayCustomerDetailsList", model);

            

        }

        [HttpGet]
        public ActionResult HtmlBeginForm(int? id)

        {
            CustomerModel model = new CustomerModel();
            try
            {
                List<CustomerModel> cusDetailsList = new List<CustomerModel>();
                customerdbEntities2 db = new customerdbEntities2();
                var cus = db.GetCustomerdb(null).ToList();
                foreach (var item in cus)
                {
                    int? Pin = null;
                    if (!string.IsNullOrEmpty(item.Pincode))
                    {
                        Pin = Convert.ToInt32(item.Pincode.Trim());
                    }
                    cusDetailsList.Add(new CustomerModel()
                    {
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Gender = item.Gender.Value,
                        DoB = item.DoB,
                        Email = item.Email,
                        PhoneNumber = item.PhoneNumber,
                        Address = item.Address,
                        City = item.City,
                        State = item.State,
                        Pincode = Pin,
                        CustomerId = item.CustomerId,
                        FollowUpCount = item.FollowUpCount
                    });
                }
                model.cusDetailsList = cusDetailsList;
                if (id != null)
                {
                    var cusDet = cusDetailsList.Where(m => m.CustomerId == id).FirstOrDefault();
                    model.FirstName = cusDet.FirstName;
                    model.LastName = cusDet.LastName;
                    model.Gender = cusDet.Gender;
                    model.DoB = cusDet.DoB;
                    model.Email = cusDet.Email;
                    model.PhoneNumber = cusDet.PhoneNumber;
                    model.Address = cusDet.Address;
                    model.City = cusDet.City;
                    model.State = cusDet.State;
                    model.Pincode = cusDet.Pincode;
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult HtmlBeginForm(CustomerModel model)
        {
            customerdbEntities2 db = new customerdbEntities2();
            string message = string.Empty;
            bool status = false;
            try
            {
                if (model.DoB.Value.Year <= 2000)
                {

                    if (!model.CustomerId.HasValue)
                    {
                        //var filename = Server.MapPath("~/CustomerImages/" + model.ImageFile.FileName);
                        string filename = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                        if (model.ImageFile.ContentLength < 1000000)
                        {
                            if (model.ImageFile.ContentType == "image/png" || model.ImageFile.ContentType == "image/jpeg" || model.ImageFile.ContentType == "image/jpg")
                            {
                                model.ImagePath = "~/CustomerImages/";
                                model.ImageFile.SaveAs(Path.Combine(Server.MapPath("~/CustomerImages/"), filename));
                                //model.ImageFile.SaveAs(filename);
                                db.SaveCustomerdb(model.FirstName, model.LastName, model.Gender, model.DoB, model.Email, model.PhoneNumber, model.Address, model.City, model.State, model.Pincode.ToString(), null, model.ImageFile.FileName);
                                message = "Record saved successfully.";
                                status = true;
                            }
                            else
                            {
                                Response.Write("<h1 style='color: Tomato;'><marquee>Upload Failed!Supports Only JPEG and PNG Format.</marquee></h1>");
                            }
                        }
                        else
                        {

                            Response.Write("<h1 style='color: Tomato;'><marquee>Upload Failed!File size must be below 1Mb.</marquee></h1>");
                            }
                    }
                    else
                   {
                        if (model.ImageFile == null)
                        {
                            var fileNames = Path.GetFileName(model.ImagePath);
                            db.UpdateCustomerdb(model.CustomerId, model.FirstName, model.LastName, model.Gender, model.DoB, model.Email, model.PhoneNumber, model.Address, model.City, model.State, model.Pincode.ToString(), null, fileNames);
                            message = "Record updated successfully.";
                            status = true;
                        }
                
                else
                {
                    if (model.ImageFile.ContentLength < 1000000)
                    {
                        if (model.ImageFile.ContentType == "image/png" || model.ImageFile.ContentType == "image/jpeg" || model.ImageFile.ContentType == "image/jpg")
                        {
                                    
                            db.UpdateCustomerdb(model.CustomerId, model.FirstName, model.LastName, model.Gender, model.DoB, model.Email, model.PhoneNumber, model.Address, model.City, model.State, model.Pincode.ToString(), null, model.ImageFile.FileName);
                            message = "Record updated successfully.";
                            status = true;
                        }
                        else
                        {
                            Response.Write("<h1 style='color: Tomato;'><marquee>Upload Failed!Supports Only JPEG and PNG Format.</marquee></h1>");
                        }
                    }
                    else
                    {
                        Response.Write("<h1 style='color: Tomato;'><marquee>Upload Failed!File size must be below 1Mb.</marquee></h1>");
                    }
                }
                    }
                }
                else
                {
                    message = "Date error, Please try again.";
                    status = false;
                }
                List<CustomerModel> cusDetailsList = new List<CustomerModel>();
                var cus = db.GetCustomerdb(null).ToList();
                foreach (var item in cus)
                {
                    cusDetailsList.Add(new CustomerModel()
                    {
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Gender = item.Gender.Value,
                        DoB = item.DoB,
                        Email = item.Email,
                        PhoneNumber = item.PhoneNumber,
                        Address = item.Address,
                        City = item.City,
                        State = item.State,
                        Pincode = Convert.ToInt32(item.Pincode),
                        CustomerId = item.CustomerId,
                        ImagePath= item.ImagePath
                    });
                }
                model.cusDetailsList = cusDetailsList;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                message = "ERROR! Please Try Again";
                status = false;
            }

            return RedirectToAction("DisplayCustomerDetailsList"); 
        }
        [HttpGet]
        public ActionResult EditTestingImage(int? id)
        {
            customerdbEntities2 db = new customerdbEntities2();
            CustomerModel model = new CustomerModel();
            if (id != null)
            {
                var Cusdet = db.GetCustomerdb(id).FirstOrDefault();
                if (Cusdet != null)
                {
                    model.CustomerId = Cusdet.CustomerId;
                    model.FirstName = Cusdet.FirstName;
                    model.LastName = Cusdet.LastName;
                    model.Gender = Cusdet.Gender.Value;
                    model.DoB = Cusdet.DoB;
                    model.Email = Cusdet.Email;
                    model.PhoneNumber = Cusdet.PhoneNumber;
                    model.Address = Cusdet.Address;
                    model.City = Cusdet.City;
                    model.State = Cusdet.State;
                    model.Pincode = Convert.ToInt32(Cusdet.Pincode);
                    model.ImagePath = "~/CustomerImages/" + Cusdet.ImagePath;

                }
            }
            return View("HtmlBeginForm",model);
        }
        [HttpPost]
        public ActionResult HtmlBeginDeleteCustomer(int id)
        {
            CustomerModel model = new CustomerModel();

            customerdbEntities2 db = new customerdbEntities2();

            string message = string.Empty;
            bool status = false;
            try
            {
                var Cusdets = db.GetCustomerdb(id).FirstOrDefault();
                var filepath = Request.MapPath("~/CustomerImages/" + Cusdets.ImagePath);

                System.IO.File.SetAttributes(filepath, FileAttributes.Normal);
                System.IO.File.Delete(filepath);


                //Delete All Files In Folder
                //System.IO.DirectoryInfo folderInfo = new DirectoryInfo(folderPath);
                //    foreach (FileInfo file in folderInfo.GetFiles())
                //    {
                //    file.Delete();
                //}
                //foreach (DirectoryInfo dir in folderInfo.GetDirectories())
                //{
                //    dir.Delete(true);

                var Cusdet = db.DeleteCustomerdb(id);
                var cus = db.GetCustomerdb(null).ToList();


                List<CustomerModel> cusDetailsList = new List<CustomerModel>();
                foreach (var item in cus)
                {
                    cusDetailsList.Add(new CustomerModel()
                    {
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Gender = item.Gender.Value,
                        DoB = item.DoB,
                        Email = item.Email,
                        PhoneNumber = item.PhoneNumber,
                        Address = item.Address,
                        City = item.City,
                        State = item.State,
                        Pincode = Convert.ToInt32(item.Pincode),
                        CustomerId = item.CustomerId
                    });
                }
                model.cusDetailsList = cusDetailsList;
                message = "Record deleted successfully.";
                status = true;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                message = "Error! This User Has Followup.";
                status = false;
            }  
            return View("DisplayCustomerDetailsList",model);
        }
        [HttpPost]
        public ActionResult UploadFiles(CustomerModel model)
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        string filename = Path.GetFileName(Request.Files[i].FileName);
                        HttpPostedFileBase file = files[i];
                        string fname;
 
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }
                      fname = Path.Combine(Server.MapPath("~/CustomerImages/"), fname);
                        file.SaveAs(fname);
                        var fileNames = Path.GetFileName(fname.TrimStart());
                        db.SaveCustomerdb(model.FirstName,model.LastName, model.Gender, model.DoB, model.Email, model.PhoneNumber, model.Address, model.City, model.State, model.Pincode.ToString(), null, fileNames);
                    }
                return Json("File Uploaded Successfully!");
            }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
        }
            else
            {
                return Json("No files selected.");
    }
}
        [HttpGet]
        public ActionResult Search()

        {
            customerdbEntities2 entities = new customerdbEntities2();
            return View(entities.SearchCustomers("int CustomerId" , "string LastName"));


        }
        [HttpPost]
        public ActionResult Search(string FirstName, string LastName)
        {
            customerdbEntities2 entities = new customerdbEntities2();
            return View(entities.SearchCustomers(FirstName,LastName));
        }

        //[HttpPost]
        //public void ProcessRequest(HttpContext context)
        //{
        //    string term = context.Request["term"] ?? "";
        //    List<string> listCustomerNames = new List<string>();
        //    string cs = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
        //    using (SqlConnection con = new SqlConnection(cs))
        //        {
        //        SqlCommand cmd = new SqlCommand("spGetStudentName", con);
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        SqlParameter parameter = new SqlParameter()
        //        {
        //            ParameterName = "@term",
        //            Value = term
        //        };

        //        cmd.Parameters.Add(parameter);
        //        con.Open();
        //        SqlDataReader rdr = cmd.ExecuteReader();
        //        while (rdr.Read())
        //        {
        //            listCustomerNames.Add(rdr["FirstName"].ToString());
        //        }
        //    }

        //}

    }
}
