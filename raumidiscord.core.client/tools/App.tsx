import { useState, useEffect, ChangeEvent, } from "react";
import axios from "axios";
import * as React from 'react';
import { useParams } from 'react-router-dom';
import { Fab, TextField } from "@mui/material";
import AddIcon from "@mui/icons-material/Add";
import Select from '@mui/material/Select';
import MenuItem from '@mui/material/MenuItem';
import InputLabel from '@mui/material/InputLabel';
import FormControl from '@mui/material/FormControl';
import dayjs, { Dayjs } from 'dayjs';
import utc from 'dayjs/plugin/utc';
import timezone from 'dayjs/plugin/timezone';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { DateTimePicker } from '@mui/x-date-pickers/DateTimePicker';
import SendIcon from '@mui/icons-material/Send';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TablePagination from '@mui/material/TablePagination';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Paper from '@mui/material/Paper';
import { TableVirtuoso, TableComponents } from 'react-virtuoso';
import Link from "@mui/material/Link";

type UrlCodes = {
    id?: number;
    url: string;
    urlType: string;
    ttl: string;
    discordUser: number;
};

type Props = {
    text: string;
    onChange: (e: ChangeEvent<HTMLInputElement>) => void;
    onClick: () => Promise<void>;
};

// UTCプラグインを読み込み
dayjs.extend(utc);
// timezoneプラグインを読み込み
dayjs.extend(timezone);
// 日本語化
dayjs.locale('ja');
// タイムゾーンのデフォルトをJST化
dayjs.tz.setDefault('Asia/Tokyo');



const initialValues = [
    {
        id: 1,
        url: "https://x.com",
        urlType: "url",
        ttl: "2001-01-01 01:00:00",
    },
    {
        id: 2,
        url: "https://t.co",
        urlType: "url",
        ttl: "2001-01-01 01:00:00",
    },
];

interface Column {
    id: 'url' | 'urlType' | 'ttl' | 'discordUser';
    label: string;
    minWidth?: number;
    align?: 'right';
    format?: (value: number) => string;
    renderCell?: (value: any, row?: any) => React.ReactNode;
}

const columns: Column[] = [
    {
        id: 'url',
        label: 'URL',
        minWidth: 200,
        renderCell: (value, row) => (
            <Link href={value} target="_blank" rel="noopener noreferrer">
                {value}
            </Link>
        ),
    },
    {
        id: 'urlType',
        label: 'urlType',
        minWidth: 50,
    },
    {
        id:'ttl',
        label: '有効期限',
        minWidth: 100,
        renderCell: (value) => {
            const localDate = dayjs.tz(value,'utc').tz().format('YYYY/MM/DD HH:mm:ss');

            return localDate;
        },
    },
    {
        id:'discordUser',
        label: 'DiscordUser',
        minWidth: 100,
        renderCell: (value) => {
            // Map Discord user IDs to usernames
            const userMap = {
                "558636367106539521": "raumi2019"
            };
            
            return userMap[value] || value;
        },
    }
];

const rows = [];

function createData(
    url: string,
    urltype: string,
    ttl: string,
    discordUser: string
): UrlCodes {
    
    return {
        url, urltype, ttl, discordUser };
}

function getQueryParams() {
    const params = new URLSearchParams(window.location.search);
    const urlParam = params.get('url') || '';
    const urlTypeParam = params.get('urlType') || 'URL';
    const ttlParam = params.get('ttl') || '';
    
    return { urlParam, urlTypeParam, ttlParam };
}

export const App = () => {

    const [urlCodes, setUrlCodes] = useState<UrlCodes[]>([]);
    const [page, setPage] = React.useState(0);
    const [rowsPerPage, setRowsPerPage] = React.useState(10);

    const handleChangePage = (event: unknown, newPage: number) => {
        setPage(newPage);
    };

    const handleChangeRowsPerPage = (event: React.ChangeEvent<HTMLInputElement>) => {
        setRowsPerPage(+event.target.value);
        setPage(0);
    };

    const search = useParams().search;
    const query = new URLSearchParams(search);
 

    // テキストボックスの文字列を管理するstate
    const [text, setText] = useState("");

    const [urltype, setUrltype] = React.useState('');

    // ページロード時にURLパラメータを処理
    useEffect(() => {
        const { urlParam, urlTypeParam, ttlParam } = getQueryParams();
    
        // URLパラメータが存在する場合、フォームに設定
        if (urlParam) {
            setText(urlParam);
        }
    
        if (urlTypeParam) {
            setUrltype(urlTypeParam);
        }
    
        if (ttlParam) {
            try {
                setValue(dayjs(ttlParam));
            } catch (e) {
                console.error("Invalid date format in URL parameter:", e);
            }
        }
    
        // 自動的に追加処理を行うかどうかのパラメータ(autoAdd=true)がある場合
        const autoAdd = new URLSearchParams(window.location.search).get('autoAdd');
        if (autoAdd === 'true' && urlParam) {
            // URLパラメータがある場合に自動追加
            handleAdd();
        }
    }, []);

    // テキストボックス入力時の処理
    const handleChangeInput = (e: React.ChangeEvent<HTMLInputElement>) => {
        // テキストボックスの文字列をstateにセット
        setText(e.target.value);
    };

    const handleAdd = async () => {
        // 日時がnullの場合のデフォルト値を設定
        const timeLimit = value ? value.format('YYYY-MM-DDTHH:mm:ss') : dayjs.utc().add(7, 'day').format('YYYY-MM-DDTHH:mm:ss');
    
        // 新しいアイテムのオブジェクトを作成（idはDB側で自動採番するため省略）
        const newCodes = { url: text, urlType: urltype, timeLimit: timeLimit };

        try {
            // APIにPOSTリクエストし、レスポンスから登録したアイテムオブジェクトを取り出す
            const { data } = await axios.post('../api/UrlDataModels', newCodes);

            // 既存のアイテムと新規登録したアイテムを合体させてstateにセット
            setUrlCodes([...urlCodes, data]);
      
            // 追加後に履歴をきれいにする（URLパラメータを削除）
            if (window.location.search) {
                window.history.replaceState({}, document.title, window.location.pathname);
            }
        } catch (e) {
            console.error(e);
        }
        // テキストボックスをクリア
        setText("");
    };

    // 完了ステータス（チェックボックス）変更時の処理
    const handleChangeStatus = async (id?: number) => {
        // 対象のアイテムの完了フラグを反転して新しい配列に格納
        const newCodes = urlCodes.map((code) => {
            if (code.id === id) {
                //code.isComplete = !code.isComplete;
            }
            return code;
        });

        // 更新対象のアイテムを取得
        const targetCode = newCodes.filter((code) => code.id === id)[0];

        try {
            // APIに更新対象のアイテムをPUTリクエスト
            await axios.put(`../api/UrlDataModels/${id}`, targetCode);

            // 新しい配列をstateにセット
            setUrlCodes(newCodes);
        } catch (e) {
            console.error(e);
        }
    };

    // ページ初期表示時の処理
    useEffect(() => {
        // APIからデータを取得する関数を定義
        const fetchUrlData = async () => {
            try {
                // APIにGETリクエストし、レスポンスからアイテムオブジェクトの配列を取り出す
                const { data } = await axios.get('../api/UrlDataModels');
                console.log(data)
                // stateにセット
                setUrlCodes(data);
            } catch (e) {
                console.error(e);
            }
        }
        // 関数の実行
        fetchUrlData();
    }, []);

    const contents = urlCodes === undefined
        ? <p><em>サーバーへの準備できていません。サーバーの準備ができてから再度読み込みなおしてください。</em></p>
        : <p>connected</p>;

    const handleChangeSelect = (event: SelectChangeEvent) => {
        setUrltype(event.target.value);
    };

    const [value, setValue] = React.useState<Dayjs | null>(
        dayjs.utc(),
    );
    

    return (
        <div>
            <h1 id="tableLabel">HoYoTool</h1>
            <p>このコンポーネントは、サーバーからデータを取得しています。</p>
            <div>
                <TextField
                    sx={{ width: "100%", maxWidth: 270, marginRight: 2, marginBottom: 2 }}
                    required
                    size="small"
                    id="standard-basic"
                    label="URL/Code"
                    variant="standard"
                    onChange={handleChangeInput}
                    value={text}
                />
                <FormControl>
                    <InputLabel id="demo-simple-select-label">URLType</InputLabel>
                    <Select
                        sx={{ minWidth: 200, marginRight: 2, marginBottom: 2 }}
                        size="small"
                        labelId="demo-simple-select-label"
                        id="demo-simple-select"
                        value={urltype}
                        label="URLType"
                        onChange={handleChangeSelect}
                    >
                        <MenuItem value={"URL"}>URL</MenuItem>
                        <MenuItem value={"GI"}>原神</MenuItem>
                        <MenuItem value={"HSR"}>崩壊：スターレイル</MenuItem>
                        <MenuItem value={"ZZZ"}>ゼンレスゾーンゼロ</MenuItem>
                    </Select>
                </FormControl>
                <Fab variant="extended" size="small" color="primary" onClick={onclick} sx={{ marginRight: 2, marginBottom: 2 }}>
                    <SendIcon sx={{ mr: 1 }} />
                    追加
                </Fab>
                <div>
                    <LocalizationProvider dateAdapter={AdapterDayjs} >
                        <Stack spacing={2} sx={{ maxWidth: 270, marginRight: 2, marginBottom: 2, margintop: 2 }}>
                            <DateTimePicker format="YYYY/MM/DD HH:mm:ss" value={value} onChange={setValue} timezone="system" />
                        </Stack>
                    </LocalizationProvider>
                </div>
            </div>
            <Paper sx={{ width: '100%', overflow: 'hidden' }}>
                <TableContainer sx={{ maxHeight: 440 }}>
                    <Table stickyHeader aria-label="sticky table">
                        <TableHead>
                            <TableRow>
                                {columns.map((column) => (
                                    <TableCell
                                        key={column.id}
                                        align={column.align}
                                        style={{ minWidth: column.minWidth }}
                                    >
                                        {column.label}
                                    </TableCell>
                                ))}
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {urlCodes
                                .slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage)
                                .map((row) => {
                                    return (
                                        <TableRow hover role="checkbox" tabIndex={-1} key={row.id || row.url}>
                                            {columns.map((column) => {
                                                const value = row[column.id];
                                                return (
                                                    <TableCell key={column.id} align={column.align}>
                                                        {column.renderCell ? column.renderCell(value, row) : (
                                                            column.format && typeof value === 'number'
                                                                ? column.format(value)
                                                                : value
                                                        )}
                                                    </TableCell>
                                                );
                                            })}
                                        </TableRow>
                                    );
                                })}
                        </TableBody>
                    </Table>
                </TableContainer>
                <TablePagination
                    rowsPerPageOptions={[10, 25, 50, 100]}
                    component="div"
                    count={urlCodes.length}
                    rowsPerPage={rowsPerPage}
                    page={page}
                    onPageChange={handleChangePage}
                    onRowsPerPageChange={handleChangeRowsPerPage}
                />
            </Paper>
        </div>
    );
};

export default App;