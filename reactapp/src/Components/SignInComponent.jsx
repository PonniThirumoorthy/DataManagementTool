import React, { useState } from 'react';
import Button from '@mui/material/Button';
import TextField from '@mui/material/TextField';
import axios from 'axios';
import RegisterComponent from './RegisterComponent';
import Avatar from '@mui/material/Avatar';
import CssBaseline from '@mui/material/CssBaseline';
import Link from '@mui/material/Link';
import Grid from '@mui/material/Grid';
import Box from '@mui/material/Box';
import LockOutlinedIcon from '@mui/icons-material/LockOutlined';
import Typography from '@mui/material/Typography';
import Container from '@mui/material/Container';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import { useHistory } from "react-router-dom";
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import Loading from './LoadingComponent';


const defaultTheme = createTheme();

const SignIn = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [isAuth, setIsAuth] = useState(sessionStorage.getItem("isAuth") || false);
    const [isLoading, setIsLoading] = useState(false);

    const history = useHistory();

    const handleSignIn = (e) => {
        setIsLoading(true);
        e.preventDefault();

        const user = {
            email: email,
            password: password
        };

        axios
            .post('https://csvdatamanagementtool.azurewebsites.net/signin', user)
            .then((response) => {
                setIsLoading(false);
                if (response.data.isSuccess) {
                    sessionStorage.setItem('isAuth', true);
                    setIsAuth(true);
                    toast.success('Sign-in successful.');
                    history.push('/main');
                    console.log('Logged in!');
                }
                else {
                    sessionStorage.setItem('isAuth', false);
                    toast.error('Error: ' + response.data.errorMessage);
                    setIsAuth(false);
                }
            })
            .catch((error) => {
                setIsLoading(false);
                toast.error("Unable to login. " + error);
                console.error(error);
            });
    };

    return (
        <>
            <div>
                {isLoading ? (
                    <Loading loading={isLoading} />
                ) : (
                    <>
                        <ToastContainer
                            position="top-right"
                            autoClose={5000}
                            hideProgressBar={false}
                            newestOnTop={false}
                            closeOnClick
                            rtl={false}
                            pauseOnFocusLoss
                            draggable
                            pauseOnHover
                            theme="colored"
                        />
                        <ThemeProvider theme={defaultTheme}>
                            <Container component="main" maxWidth="xs">
                                <CssBaseline />
                                <Box
                                    sx={{
                                        marginTop: 8,
                                        display: 'flex',
                                        flexDirection: 'column',
                                        alignItems: 'center',
                                    }}
                                >
                                    <Avatar sx={{ m: 1, bgcolor: 'secondary.main' }}>
                                        <LockOutlinedIcon />
                                    </Avatar>
                                    <Typography component="h1" variant="h5">
                                        Sign in
                                    </Typography>
                                    <Box component="form" noValidate sx={{ mt: 1 }} onSubmit={handleSignIn}>


                                        <TextField
                                            margin="normal"
                                            required
                                            fullWidth
                                            id="email"
                                            label="Email Address"
                                            name="email"
                                            autoComplete="email"
                                            value={email}
                                            onChange={(e) => setEmail(e.target.value)}
                                            autoFocus
                                        />
                                        <TextField
                                            margin="normal"
                                            required
                                            fullWidth
                                            name="password"
                                            label="Password"
                                            type="password"
                                            id="password"
                                            value={password}
                                            autoComplete="current-password"
                                            onChange={(e) => setPassword(e.target.value)}
                                        />
                                        <Button
                                            type="submit"
                                            fullWidth
                                            variant="contained"
                                            sx={{ mt: 3, mb: 2 }}
                                        >
                                            Sign In
                                        </Button>
                                        <Grid container>
                                            <Grid item>
                                                <Link to={<RegisterComponent />} href="/register" variant="body2">
                                                    {"Don't have an account? Sign Up"}
                                                </Link>
                                            </Grid>
                                        </Grid>
                                    </Box>
                                </Box>
                            </Container>
                        </ThemeProvider>
                    </>
                )}
            </div>
        </>

    );
};

export default SignIn;