import { BrowserRouter, Route, Switch, Link } from "react-router-dom";
import React, { useState } from 'react';
import Main from './Components/MainComponent';
import Register from './Components/RegisterComponent';
import SignIn from './Components/SignInComponent';
import ProtectedRoute from './Components/ProtectedRouteComponent';
import useAuth from "./CustomHooks/useAuth";
import NavBar from "./Components/NavBarComponent";

const App = () => {
    /*const [isAuth, login, logout] = useAuth(false);*/
    const [isAuth, setIsAuth] = useState(sessionStorage.getItem("isAuth") || false);

    window.onload = function () {
        sessionStorage.clear();
    }

    return (
        <BrowserRouter forceRefresh={true}>

            <NavBar></NavBar>
            {/* {isAuth ? (<><div className="ui message brown">You are logged in..</div><button className="ui button blue" onClick={logout}>Logout</button></>) : (<><div className="ui message violet">You are logged out..</div><button className="ui button blue" onClick={login}>Login</button></>)}*/}

            <Switch>
                <Switch>
                    <Route path="/" exact component={SignIn} />
                    <Route path="/signin" component={SignIn} />
                    <Route path="/register" component={Register} />
                    <ProtectedRoute path="/main" component={Main} auth={isAuth} />
                </Switch>
            </Switch>
        </BrowserRouter>
    );
};

export default App;

