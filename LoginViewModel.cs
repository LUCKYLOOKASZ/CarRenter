namespace CarRenter.Models
{
    public class LoginViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
@{
    ViewData["Title"] = "Home Page";
}
@model CarRenter.Models.LoginViewModel

<form asp-action = "Login" method="post">  
    <div>  
        <label>Username:</ label >
        < input asp -for= "Username" class= "form-control" />
    </ div >
    < div >
        < label > Password:</ label >
        < input asp -for= "Password" type = "password" class= "form-control" />
    </ div >
    < button type = "submit" class= "btn btn-primary" > Login </ button >
</ form >
