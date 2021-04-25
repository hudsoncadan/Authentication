// response_mode is required because we are using pkce
let options = {
    response_mode: "query"
};

var userManager = new Oidc.UserManager(options);

userManager.signinCallback()
    .then(res => {
        console.log(res);
        window.location.href = "/home/index";
    });
