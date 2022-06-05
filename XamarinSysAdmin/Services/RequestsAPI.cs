﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Xamarin.Forms;
using XamarinSysAdmin.Models;
using XamarinSysAdmin.Views;

namespace XamarinSysAdmin.Services
{
    class RequestsAPI
    {
        private static RequestsAPI Context { get; set; }
        public static RequestsAPI get()
        {
            if (Context == null) Context = new RequestsAPI();
            return Context;
        }

      
        private string ip
        {
            get
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.Android:
                        return "10.0.2.2:53187";


                    case Device.UWP:
                        return "localhost:53187";

                }
                return null;
            }
        }
        
        private  WebClient client = new WebClient();
        
        private  List<T> Request<T>(string apiPath)
        {
            var b = client.DownloadString($"http://{ip}/api/{apiPath}" );
            return JsonConvert.DeserializeObject<List<T>>(b);
        }
        private T Request<T>(string apiPath,bool bo)
        {
            var b = client.DownloadString($"http://{ip}/api/{apiPath}");
            return JsonConvert.DeserializeObject<T>(b);
        }


        public List<Users> SelectUsers()
        {
            
            var b = client.DownloadString($"http://{ip}/api/Users");
            List<Users> users = JsonConvert.DeserializeObject<List<Users>>(b);
                return users;           
        }

        public List<Spec> SelectSpec()
        {
           return Request<Spec>("Specs");
        }

        public Storage SelectStorage(int id)
        {
            return (Request<Storage>($"Storages/{id}",false));
        }

        public List<Quire> SelectQuire0()
        {
            var list = Request<Quire>("Quires");


            
            List<Quire> lQ = new List<Quire>();
            foreach (var a in list.ToList())
            {
                if ( a.UserId == Personal.Session.Id)
                {
                    lQ.Add(a);
                }
            }
            var select = lQ.OrderByDescending(q => q.Date1).ToList();
            return select;
        }

        public bool InsertQuire(Quire q)
        {
            try
            {
                var Client = new WebClient();
                Client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                var result = Client.UploadString($"http://{ip}/api/Quires", "POST", JsonConvert.SerializeObject(q));
                return true;
            }
            catch
            {
                return false;
            }
           
           

        }
        /// <summary>
        /// Добавляет материал к заявке
        /// </summary>
        /// <param name="ml"></param>
        /// <returns></returns>
        public bool InsertMaterialList(MaterialList ml)
        {
            try 
            {
                var Client = new WebClient();
                Client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                var result = Client.UploadString($"http://{ip}/api/materiallists", "POST", JsonConvert.SerializeObject(ml));

              

                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Удаляет материал из списка материалов 
        /// </summary>
        /// <param name="DeletedMaterialList">Удаляемый объект</param>
        public void DeleteMaterialList(MaterialList DeletedMaterialList)
        {
            int Id = DeletedMaterialList.IdList;
            var Client = new WebClient();
            Client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            var result = Client.UploadString($"http://{ip}/api/materiallists/{Id}", "DELETE", JsonConvert.SerializeObject(Id));
            // удалили
            // должны вернуть материал
            UpdateStorage(SelectStorage(DeletedMaterialList.MaterId.Value),-DeletedMaterialList.AmountInList.Value);
            
        }

        public bool UpdateStorage(Storage s,int i)
        {
            try
            {
                s.Amount -= i;
                var Client = new WebClient();
                Client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                var result = Client.UploadString($"http://{ip}/api/Storages/{s.IdMaterial}", "PUT", JsonConvert.SerializeObject(s)
                    );
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Quire> SelectQuireSpec()
        {
            var list = Request<Quire>("Quires");



            List<Quire> lQ = new List<Quire>();
            foreach (var a in list.ToList())
            {
                if (a.Status == 1 && a.SpecId== Personal.Session.Id)
                {
                    lQ.Add(a);
                }
            }
            var select = lQ.OrderByDescending(q => q.Date1).ToList();
            return select;
        }

        public List<Quire> SelectQuireAdmin()
        {
           return Request<Quire>("getQuireAdmin");       
        }

        public List<Spec> SelectOnlySpec()
        {
            return Request<Spec>("getOnlySpec");
        }

        public List<Storage> SelectStorage()
        {
            return Request<Storage>("Storages");
        }

        public List<MaterialList> SelectMaterialList()
        {
            var list = Request<MaterialList>("materiallists");



            List<MaterialList> materialLists = new List<MaterialList>();
            foreach (var a in list.ToList())
            {
                if (a.ReqId == OrderSpecDetalPage.quire.IdQuire)
                {
                    materialLists.Add(a);
                }
            }
          
            return materialLists;
        }

        public bool UpdateQuire(Quire q,string ans)
        {
            try
            {
                q.Status = 2;
                q.Date2 = DateTime.Now;
                q.Answer = ans;
                var Client = new WebClient();
                Client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                var result = Client.UploadString($"http://{ip}/api/Quires/{q.IdQuire}", "PUT", JsonConvert.SerializeObject(q)
                    );
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateQuireAdmin(Quire q, Spec s)
        {
            try
            {
                q.Status = 1;
                q.SpecId = s.IdSpec;
               
                var Client = new WebClient();
                Client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                var result = Client.UploadString($"http://{ip}/api/Quires/{q.IdQuire}", "PUT", JsonConvert.SerializeObject(q)
                    );
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}