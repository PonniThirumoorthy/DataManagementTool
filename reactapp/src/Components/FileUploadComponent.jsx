import { Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Paper } from '@mui/material';
import Button from '@mui/material/Button';
import Stack from '@mui/material/Stack';
import Card from '@mui/material/Card';
import UploadFileIcon from '@mui/icons-material/UploadFile';
import Typography from '@mui/material/Typography';
import Box from '@mui/material/Box';
import Container from '@mui/material/Container';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import Loading from './LoadingComponent';

import React, { useState, useEffect } from "react";
import axios from "axios";
import { useHistory } from "react-router-dom";

const FileUpload = ({ setloading }) => {
    const [selectedFile, setFileSelected] = useState();
    const [isFilePicked, setFilePicked] = useState(false);
    const [isLoading, setIsLoading] = useState(false);
    const [isAuth, setIsAuth] = useState(sessionStorage.getItem("isAuth") || false);

    const history = useHistory();

    const onFileChange = (event) => {
        setFileSelected(event.target.files[0]);
        setFilePicked(true);
        sessionStorage.setItem('FileName', event.target.files[0].name);
    }

    const onUploadButtonClick = (e) => {

        const formData = new FormData();
        sessionStorage.setItem('FileName', selectedFile.name);
        formData.append('file', selectedFile)

        try {
            let config = {
                method: 'post',
                maxBodyLength: Infinity,
                //    url: 'https://localhost:7178/FileUpload/ImportFile',
                url: 'https://csvdatamanagementtool.azurewebsites.net/FileUpload/ImportFile',
                data: formData,
                Headers: {
                    'content-type': 'multipart/form-data'
                }
            };
            setIsLoading(true);
            axios.request(config)
                .then((response) => {
                    debugger;
                    console.log(JSON.stringify(response.data));
                    if (response.data.isSuccess) {
                        toast.success('File upload was successful.');
                        //alert('File upload was successful.');
                    }
                    else {
                        toast.error('Error uploading file. ' + response.data.errorMessage);
                    }
                    setIsLoading(false);
                })
                .catch((error) => {
                    setIsLoading(false);
                    console.log(error);
                    toast.error('Error uploading file - ' + error);
                });

            //const res = await axios.post("https://localhost:7004/FileUpload/ImportFile", formData);

        } catch (ex) {
            console.log(ex);
        }
    };

    function formatBytes(bytes, decimals = 2) {
        if (!+bytes) return '0 Bytes'

        const k = 1024
        const dm = decimals < 0 ? 0 : decimals
        const sizes = ['Bytes', 'KiB', 'MiB', 'GiB', 'TiB', 'PiB', 'EiB', 'ZiB', 'YiB']

        const i = Math.floor(Math.log(bytes) / Math.log(k))

        return `${parseFloat((bytes / Math.pow(k, i)).toFixed(dm))} ${sizes[i]}`
    }

    if (isAuth) {
        return (
            <>
                <ToastContainer
                    position="top-right"
                    autoClose={3000}
                    hideProgressBar={false}
                    newestOnTop={false}
                    closeOnClick
                    rtl={false}
                    pauseOnFocusLoss
                    draggable
                    pauseOnHover
                    theme="colored"
                />
                <Loading loading={isLoading} />
                <Box
                    sx={{
                        bgcolor: 'background.paper',
                        pt: 4,
                        pb: 3,
                    }}
                >
                    <Container maxWidth="sm">

                        <Typography
                            component="h2"
                            variant="h4"
                            align="center"
                            color="text.primary"
                            gutterBottom
                        >
                            CSV File Import
                        </Typography>

                        <Stack
                            sx={{ pt: 4 }}
                            direction="row"
                            spacing={2}
                            justifyContent="center"
                        >
                            <div className="row">
                                <div className="col">
                                    <input accept='.csv' className="form-control" id="formFileLg" type="file" name="file" onChange={onFileChange} />
                                </div>
                                <div className="col">
                                    <Button variant="contained" onClick={onUploadButtonClick} endIcon={<UploadFileIcon />}>Upload File</Button>
                                </div>
                            </div>
                            <br />
                        </Stack>
                        <div className="row">
                            {isFilePicked ? (
                                <div>
                                    <br />
                                    <TableContainer component={Paper}>
                                        <Table>
                                            <TableBody>
                                                <TableRow>
                                                    <TableCell> <b>File Name: </b></TableCell>
                                                    <TableCell>{selectedFile.name}</TableCell>
                                                </TableRow>
                                                <TableRow>
                                                    <TableCell><b>File Size:</b></TableCell>
                                                    <TableCell>{formatBytes(selectedFile.size)}</TableCell>
                                                </TableRow>
                                                <TableRow>
                                                    <TableCell><b>File Last Modified:</b></TableCell>
                                                    <TableCell>{selectedFile.lastModifiedDate.toLocaleDateString()}</TableCell>
                                                </TableRow>
                                            </TableBody>
                                        </Table>
                                    </TableContainer>
                                </div>
                            ) : (
                                <p></p>
                            )}
                        </div>
                    </Container>
                </Box>
            </>
        );
    }
    else {
        history.push('/signin');
    }
}

export default FileUpload;

