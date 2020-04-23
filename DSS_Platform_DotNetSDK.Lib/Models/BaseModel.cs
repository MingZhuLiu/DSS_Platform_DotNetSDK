using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSS_Platform_DotNetSDK.Lib.Models
{
    public class BaseModel<T>
    {
        public bool Flag { get; set; }
        public String Msg { get; set; }
        public T Data { get; set; }

        public BaseModel()
        {

        }
        public BaseModel(bool flag, string msg)
        {
            this.Flag = flag;
            this.Msg = msg;
        }
        public BaseModel<T> Success(string msg,T data)
        {
            this.Flag = true;
            this.Msg = msg;
            this.Data=data;
            return this;
        }
          public BaseModel<T> Success(T data)
        {
            this.Flag = true;
            this.Msg = "ok";
            this.Data=data;
            return this;
        }

        public BaseModel<T> Failed(Exception ex)
        {
            this.Flag = false;
            this.Msg = ex.Message;
            return this;
        }
        public BaseModel<T> Failed(string msg)
        {
            this.Flag = false;
            this.Msg = msg;
            return this;
        }
    }

}
