import React, { useState, useEffect } from 'react';
import FileProcess from './FileProcessComponent';
import FileUpload from './FileUploadComponent';
import Loading from './LoadingComponent';
import { useHistory } from "react-router-dom";

function Main() {
    const [isAuth, setIsAuth] = useState(sessionStorage.getItem("isAuth") || false);
    const [isLoading, setIsLoading] = useState(false); 
    const history = useHistory();

    useEffect(() => {
        window.onbeforeunload = function () {
            sessionStorage.clear();
        };

        return () => {
            window.onbeforeunload = null;
        };
    }, []);

    const loadingfunc = (isSet) => {
        setIsLoading(isSet);
    }

    if (!isAuth) {
        history.push('/signin');
    }
    else {
        return (
            <>
                <Loading loading={isLoading} />
                <FileUpload loading={loadingfunc}></FileUpload>
                <FileProcess loading={loadingfunc}></FileProcess>
            </>
        );
    }
}

export default Main;