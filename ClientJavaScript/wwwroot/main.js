var config = {
    authority: "https://localhost:44353", // IdentityServer4
    client_id: "client_id_js",
    redirect_uri: "https://localhost:44307/Home/SignIn", // ClientJS
    //response_type: "id_token token",
    response_type: "code", // it has changed to code cause we are using pkce
    scope: "openid ApiOne userPermissionsScope",
    post_logout_redirect_uri: "https://localhost:44307/Home/Index"
};

var userManager = new Oidc.UserManager(config);

userManager.getUser().then(user => {
    console.log(user);
    if (user) {
        axios.defaults.headers.common["Authorization"] = "Bearer " + user.access_token;
    }
})

const signInLibrary = () => {
    userManager.signinRedirect();
}

const logoutLibrary = () => {
    userManager.signoutRedirect();
}

const callApiOne = () => {
    axios.get("https://localhost:44306/secret") // ApiOne
        .then(res => {
            console.log(res);
        });
}

var refreshing = false;

axios.interceptors.response.use(
    (response) => {
        return response;
    },
    (error) => {
        console.log(error?.response);

        let axiosConfig = error.response.config;

        // Refresh token
        if (error.response.status  === 401) {

            // It's already refreshing, don't make another request
            if (!refreshing) {
                refreshing = true;

                console.log('Start refreshing the token');

                // do the refresh
                return userManager.signinSilent()
                    .then(user => {
                        console.log('refresh user', user);

                        // Update the token
                        axios.defaults.headers.common["Authorization"] = "Bearer " + user.access_token;
                        axiosConfig.headers["Authorization"] = "Bearer " + user.access_token;

                        refreshing = false;

                        // Retry the last http request
                        return axios(axiosConfig);
                    });
            }
        }

        return Promise.reject(error?.response);
    }
);