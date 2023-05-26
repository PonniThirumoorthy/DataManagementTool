import { React, useEffect, useState } from 'react';
import Button from '@mui/material/Button';
import HourglassFullIcon from '@mui/icons-material/HourglassFull';
import Stack from '@mui/material/Stack';
import axios from 'axios';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import Loading from './LoadingComponent';
import ArrowForwardIcon from '@mui/icons-material/ArrowForward';
import { DataGrid } from '@mui/x-data-grid';
import Typography from '@mui/material/Typography';


const DataTable = ({ rows }) => {
    const fileName = sessionStorage.getItem('FileName');
    //useEffect(() => {
    //    // Fetch data from your SQL table using an API endpoint
    //    axios.get('https://localhost:7178/GetData?fileName=' + fileName)
    //        .then(response => {
    //            debugger;
    //            console.log(response.data.data);
    //            setData(response.data.data);
    //            console.log('tbData below:');
    //            console.log(tbData);

    //            setRows([
    //                {
    //                    id: 1, Validation: "NoOfEmptyRows", ValidationValue:
    //                        response.data.data.noOfEmptyRows
    //                },
    //                {
    //                    id: 2, Validation: "TotalRecords", ValidationValue:
    //                        response.data.data.totalRecords
    //                }
    //            ]);
    //        })
    //        .catch(error => {
    //            console.error(error);
    //        });
    //}, []);
    const columns = [
        { field: 'Validation', headerName: 'Validation', width: 200 },
        { field: 'ValidationValue', headerName: 'No of rows', width: 100 }
    ];
    return (
        <>
            <div style={{ height: 400, width: '50%' }}>
            <br />
                <Typography component="h3" variant="h6">
                    File Validation Report of <b> {fileName}</b>
                </Typography>
                <DataGrid
                    rows={rows}
                    autoHeight={true}
                    columns={columns}
                    hideFooterPagination={true}
                    hideFooter={true}
                    rowSpacingType='border'
                />
            </div> </>
    );
};
function FileProcess(props) {
    const selectedFile = sessionStorage.getItem('FileName');
    const [isLoading, setIsLoading] = useState(false);
    const [showTable, setShowTable] = useState(false);
    const [rows, setRows] = useState([
        {
            id: 1, Validation: "NoOfEmptyRows", ValidationValue:
                ""
        }
    ]);

    const onValButtonClick = async () => {
        setRows([
            {
                id: 1, Validation: "NoOfEmptyRows", ValidationValue:
                    ""
            }
        ])
        await axios.get('https://csvdatamanagementtool.azurewebsites.net/GetData?fileName=' + selectedFile)
            .then(response => {
                setRows([
                    {
                        id: 1, Validation: "Total Records", ValidationValue:
                            response.data.data.totalRecords
                    },
                    {
                        id: 2, Validation: "Duplicate Rows", ValidationValue:
                            response.data.data.duplicateRows
                    },
                    {
                        id: 3, Validation: "Blank Rows", ValidationValue:
                            response.data.data.noOfEmptyRows
                    },
                    {
                        id: 4, Validation: "File Empty?", ValidationValue:
                            response.data.data.isFileEmpty
                    }
                ]);
                setShowTable(true);
            })
            .catch(error => {
                console.error(error);
            });
    }


    const onButtonClick = async () => {
        setIsLoading(true);
        setShowTable(false);
        let config = {
            //method: 'post',
            //url: 'https://localhost:7178/ProcessFile',
            //data: props.fileName,
            Headers: {
                'content-type': 'application/json'
            }
        };
        const url = 'https://csvdatamanagementtool.azurewebsites.net/ProcessFile?fileName=' + sessionStorage.getItem('FileName');

        await axios.post(url, null, config).then(response => {
            console.log(JSON.stringify(response.data));
            if (response.data.isSuccess) {
                toast.success('File processing complete.!');
            }
            else {
                toast.error('Error processing file. ' + response.data.errorMessage);
            }
            setIsLoading(false);
        })
            .catch((error) => {
                console.log(error);
                setIsLoading(false);
                toast.error("Unable to process file - " + error);
            });

    }

    return (
        <div>
            <ToastContainer
                position="top-right"
                autoClose={2000}
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
            <Stack
                sx={{ pt: 4 }}
                direction="row"
                spacing={1}
                justifyContent="center"
            >
                <Button
                    size="large"
                    variant="contained" color="primary" onClick={onButtonClick} endIcon={<HourglassFullIcon />}>Process File</Button>
                <Button
                    size="large"
                    variant="contained" color="error" onClick={onValButtonClick} endIcon={<ArrowForwardIcon />}>Show Validation data</Button>
            </Stack>
            {showTable && <DataTable rows={rows} />}
        </div>
    );
}

export default FileProcess;