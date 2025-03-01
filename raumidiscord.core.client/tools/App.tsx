import { useState, useEffect, ChangeEvent, } from "react";
import axios from "axios";
import * as React from 'react';
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
    renderCell?;
}

const columns: Column[] = [
    {
        id: 'url',
        label: 'URL',
        minWidth: 200,
        // renderCell: (params) => (
        //     <Link href={params} target="_blank" rel="noopener noreferrer">
        //       {params}
        //     </Link>
        //   ),
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
    },
    {
        id:'discordUser',
        label: 'DiscordUser',
        minWidth: 100,
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

    // テキストボックスの文字列を管理するstate
    const [text, setText] = useState("");

    const [urltype, setUrltype] = React.useState('');

    // テキストボックス入力時の処理
    const handleChangeInput = (e: React.ChangeEvent<HTMLInputElement>) => {
        // テキストボックスの文字列をstateにセット
        setText(e.target.value);
    };

    const handleAdd = async () => {
        // 新しいアイテムのオブジェクトを作成（idはDB側で自動採番するため省略）
        const newCodes = { url: text, urltype: 'URL', timeLimit:'2025-03-24T15:59:59' };

        try {
            // APIにPOSTリクエストし、レスポンスから登録したTodoアイテムオブジェクトを取り出す
            const { data } = await axios.post('../api/UrlDataModels', newCodes);

            // 既存のアイテムと新規登録したアイテムを合体させてstateにセット
            setUrlCodes([...urlCodes, data]);
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

    dayjs.extend(utc);
    dayjs.extend(timezone);
    const [value, setValue] = React.useState<Dayjs | null>(
        dayjs.utc('2025-03-24T16:00:00'),
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
                    onChange={onchange}
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
                <Fab variant="extended" size="small" color="primary" onClick={onclick} sx={{marginRight: 2, marginBottom: 2 }}>
                    <SendIcon sx={{ mr: 1 }} />
                    追加
                </Fab>
                <div>
                    <LocalizationProvider dateAdapter={AdapterDayjs} >
                        <Stack spacing={2} sx={{ maxWidth: 270, marginRight: 2, marginBottom: 2, margintop: 2 }}>
                            <DateTimePicker format="YYYY/MM/DD HH:MM:ss" value={value} onChange={setValue} timezone="system" />
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
                  <TableRow hover role="checkbox" tabIndex={-1} key={row.urlType}>
                    {columns.map((column) => {
                      const value = row[column.id];
                      return (
                          <TableCell key={column.id} align={column.align} component="a" href={row.url} target="_blank">
                          {column.format && typeof value === 'number'
                            ? column.format(value)
                            : value}
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