import EditIcon from '@mui/icons-material/Edit';
import MenuIcon from '@mui/icons-material/Menu';
import SendIcon from '@mui/icons-material/Send';
import { Fab, TextField } from "@mui/material";
import Alert from '@mui/material/Alert';
import AppBar from '@mui/material/AppBar';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import FormControl from '@mui/material/FormControl';
import IconButton from '@mui/material/IconButton';
import InputLabel from '@mui/material/InputLabel';
import Link from "@mui/material/Link";
import MenuItem from '@mui/material/MenuItem';
import Modal from '@mui/material/Modal';
import Paper from '@mui/material/Paper';
import Select, { SelectChangeEvent } from '@mui/material/Select';
import Snackbar from '@mui/material/Snackbar';
import Stack from '@mui/material/Stack';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TablePagination from '@mui/material/TablePagination';
import TableRow from '@mui/material/TableRow';
import Toolbar from '@mui/material/Toolbar';
import Typography from '@mui/material/Typography';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import { DateTimePicker } from '@mui/x-date-pickers/DateTimePicker';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import axios from "axios";
import dayjs, { Dayjs } from 'dayjs';
import timezone from 'dayjs/plugin/timezone';
import utc from 'dayjs/plugin/utc';
import * as React from 'react';
import { ChangeEvent, useEffect, useState, } from "react";

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

interface Column {
    id: 'url' | 'urlType' | 'ttl' | 'discordUser' | 'actions';
    label: string;
    minWidth?: number;
    align?: 'right';
    format?: (value: number) => string;
    renderCell?: (value: any, row?: any) => React.ReactNode;
}

// URLタイプごとのベースURL定義
const URL_TYPE_BASES = {
    'GI': 'https://genshin.hoyoverse.com/ja/gift?code=',
    'HSR': 'https://hsr.hoyoverse.com/gift?code=',
    'ZZZ': 'https://zzz.hoyoverse.com/gift?code='
};

function getQueryParams() {
    const params = new URLSearchParams(window.location.search);
    const urlParam = params.get('url') || '';
    const urlTypeParam = params.get('urlType') || 'URL';
    const ttlParam = params.get('ttl') || '';
    
    return { urlParam, urlTypeParam, ttlParam };
}

// モーダルスタイル
const modalStyle = {
    position: 'absolute',
    top: '50%',
    left: '50%',
    transform: 'translate(-50%, -50%)',
    width: 360,
    bgcolor: 'background.paper',
    border: '2px solid #7bb3ee',
    boxShadow: 24,
    p: 2,
};

export const App = () => {
    // 基本のstate設定
    const [urlCodes, setUrlCodes] = useState<UrlCodes[]>([]);
    const [page, setPage] = React.useState(0);
    const [rowsPerPage, setRowsPerPage] = React.useState(10);
    const [text, setText] = useState("");
    const [urltype, setUrltype] = React.useState('URL');
    const [value, setValue] = React.useState<Dayjs | null>(dayjs.utc().add(7, 'day'));
    
    // モーダル関連のstate
    const [isAddModalOpen, setIsAddModalOpen] = useState(false);
    const [isEditModalOpen, setIsEditModalOpen] = useState(false);
    const [editingItem, setEditingItem] = useState<UrlCodes | null>(null);
    
    // 通知用state
    const [snackbar, setSnackbar] = useState({
        open: false,
        message: '',
        severity: 'success' as 'success' | 'error'
    });
    
    // バリデーション用state
    const [errors, setErrors] = useState({
        url: '',
        urlType: '',
        ttl: ''
    });

    // テーブルの列定義
    const columns: Column[] = [
        {
            id: 'url',
            label: 'URL',
            minWidth: 200,
            renderCell: (value, row) => {
                const displayUrl = row.urlType === 'URL' ? value : 
                    (URL_TYPE_BASES[row.urlType] ? URL_TYPE_BASES[row.urlType] + value : value);
                return (
                    <Link href={displayUrl} target="_blank" rel="noopener noreferrer">
                        {displayUrl}
                    </Link>
                );
            },
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
        },
        {
            id: 'actions',
            label: 'Actions',
            minWidth: 80,
            renderCell: (_, row) => (
                <Fab
                    size="small"
                    color="primary"
                    onClick={() => handleEditClick(row)}
                >
                    <EditIcon />
                </Fab>
            ),
        }
    ];

    // ページネーション処理
    const handleChangePage = (event: unknown, newPage: number) => {
        setPage(newPage);
    };

    const handleChangeRowsPerPage = (event: React.ChangeEvent<HTMLInputElement>) => {
        setRowsPerPage(+event.target.value);
        setPage(0);
    };

    // モーダル操作
    const handleOpenAddModal = () => setIsAddModalOpen(true);
    const handleCloseAddModal = () => {
        setIsAddModalOpen(false);
        resetForm();
    };
    
    const handleOpenEditModal = () => setIsEditModalOpen(true);
    const handleCloseEditModal = () => {
        setIsEditModalOpen(false);
        setEditingItem(null);
        resetForm();
    };
    
    // 編集ボタンクリック時の処理
    const handleEditClick = (item: UrlCodes) => {
        setEditingItem(item);
        setText(item.url);
        setUrltype(item.urlType);
        setValue(dayjs.tz(item.ttl, 'utc'));
        handleOpenEditModal();
    };
    
    // フォームリセット
    const resetForm = () => {
        setText("");
        setUrltype("URL");
        setValue(dayjs.utc().add(7, 'day'));
        setErrors({
            url: '',
            urlType: '',
            ttl: ''
        });
    };

    // 入力フィールド変更時のハンドラ
    const handleChangeInput = (e: React.ChangeEvent<HTMLInputElement>) => {
        setText(e.target.value);
    };

    const handleChangeSelect = (event: SelectChangeEvent) => {
        setUrltype(event.target.value);
    };

    // バリデーション関数
    const validateForm = (): boolean => {
        let valid = true;
        const newErrors = {
            url: '',
            urlType: '',
            ttl: ''
        };
        
        // URLタイプのバリデーション
        if (!urltype) {
            newErrors.urlType = 'URLタイプは必須です';
            valid = false;
        }
        
        // URLバリデーション
        if (!text) {
            newErrors.url = 'URLは必須です';
            valid = false;
        } else if (urltype === 'URL') {
            // URL形式のバリデーション
            if (!text.startsWith('http://') && !text.startsWith('https://')) {
                newErrors.url = 'URLはhttp://またはhttps://で始まる必要があります';
                valid = false;
            }
        } else {
            // コード形式のバリデーション（GI, HSR, ZZZ）
            const codeRegex = /^[A-Z0-9]+$/;
            if (!codeRegex.test(text)) {
                newErrors.url = 'コードは大文字アルファベットと数字のみ使用できます';
                valid = false;
            }
        }
        
        // 日付バリデーション
        if (!value) {
            newErrors.ttl = '有効期限は必須です';
            valid = false;
        } else if (value.isBefore(dayjs())) {
            newErrors.ttl = '有効期限は現在時刻より後である必要があります';
            valid = false;
        }
        
        setErrors(newErrors);
        return valid;
    };

    // データ追加処理
    const handleAdd = async () => {
        if (!validateForm()) {
            return;
        }
        
        // 日時がnullの場合のデフォルト値を設定
        const timeLimit = value ? value.format('YYYY-MM-DDTHH:mm:ss') : dayjs.utc().add(7, 'day').format('YYYY-MM-DDTHH:mm:ss');
        
        // URLを適切に処理
        let processedUrl = text;
        
        // もしURLタイプがURL以外の場合、ベースURLは保存せず、コードのみを保存
        // API側でベースURLと結合する必要はなし
    
        // 新しいアイテムのオブジェクトを作成
        const newCodes = { url: processedUrl, urlType: urltype, timeLimit: timeLimit };

        try {
            // APIにPOSTリクエスト
            const { data } = await axios.post('../api/UrlDataModels', newCodes);

            // 既存のアイテムと新規登録したアイテムを合体させてstateにセット
            setUrlCodes([...urlCodes, data]);
      
            // 追加後に履歴をきれいにする（URLパラメータを削除）
            if (window.location.search) {
                window.history.replaceState({}, document.title, window.location.pathname);
            }
            
            // モーダルを閉じる
            handleCloseAddModal();
            
            // 成功通知
            setSnackbar({
                open: true,
                message: '正常に追加されました',
                severity: 'success'
            });
        } catch (e) {
            console.error(e);
            // エラー通知
            setSnackbar({
                open: true,
                message: '追加中にエラーが発生しました',
                severity: 'error'
            });
        }
    };
    
    // データ更新処理
    const handleUpdate = async () => {
        if (!validateForm() || !editingItem?.id) {
            return;
        }
        
        // 日時の処理
        const timeLimit = value ? value.format('YYYY-MM-DDTHH:mm:ss') : dayjs.utc().add(7, 'day').format('YYYY-MM-DDTHH:mm:ss');
        
        // URLを適切に処理
        let processedUrl = text;
        
        // 更新対象のアイテムを作成
        const updatedItem = { 
            ...editingItem,
            url: processedUrl, 
            urlType: urltype, 
            timeLimit: timeLimit,
            ttl: timeLimit // APIの期待する形式に合わせる
        };

        try {
            // APIにPUTリクエスト
            await axios.put(`../api/UrlDataModels/${editingItem.id}`, updatedItem);

            // stateを更新
            const updatedCodes = urlCodes.map(code => 
                code.id === editingItem.id ? {...code, url: processedUrl, urlType: urltype, ttl: timeLimit} : code
            );
            setUrlCodes(updatedCodes);
            
            // モーダルを閉じる
            handleCloseEditModal();
            
            // 成功通知
            setSnackbar({
                open: true,
                message: '正常に更新されました',
                severity: 'success'
            });
        } catch (e) {
            console.error(e);
            // エラー通知
            setSnackbar({
                open: true,
                message: '更新中にエラーが発生しました',
                severity: 'error'
            });
        }
    };
    
    // スナックバーを閉じる
    const handleCloseSnackbar = () => {
        setSnackbar({...snackbar, open: false});
    };

    // ページ初期表示時の処理
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
            handleOpenAddModal();
        }
        
        // APIからデータを取得
        const fetchUrlData = async () => {
            try {
                const { data } = await axios.get('../api/UrlDataModels');
                console.log(data);
                setUrlCodes(data);
            } catch (e) {
                console.error(e);
                setSnackbar({
                    open: true,
                    message: 'データの読み込み中にエラーが発生しました',
                    severity: 'error'
                });
            }
        };
        
        fetchUrlData();
    }, []);

    return (
        <div>
            <Box sx={{ flexGrow: 1 }}>
                <AppBar position="static">
                    <Toolbar>
                        <IconButton
                            size="large"
                            edge="start"
                            color="inherit"
                            aria-label="menu"
                            sx={{ mr: 2 }}
                        >
                            <MenuIcon />
                        </IconButton>
                        <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
                            ＞
                        </Typography>
                        <Button color="inherit">Login</Button>
                    </Toolbar>
                </AppBar>
            </Box>
            <h1 id="tableLabel">HoYoTool</h1>
            <p>このコンポーネントは、サーバーからデータを取得しています。</p>
            
            {/* 追加ボタン */}
            <Fab variant="extended" size="medium" color="primary" onClick={handleOpenAddModal} sx={{ mb: 2 }}>
                <SendIcon sx={{ mr: 1 }} />
                追加
            </Fab>
            
            {/* データテーブル */}
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
            
            {/* 追加用モーダル */}
            <Modal
                open={isAddModalOpen}
                onClose={handleCloseAddModal}
                aria-labelledby="add-modal-title"
                aria-describedby="add-modal-description"
            >
                <Box sx={modalStyle}>
                    <Typography id="add-modal-title" variant="h6" component="h2">
                        新しく追加する
                    </Typography>
                    <Typography id="add-modal-description" sx={{ mt: 2 }}>
                        <div>
                            <TextField
                                sx={{ width: "100%", maxWidth: 270, marginRight: 2, marginBottom: 2 }}
                                required
                                id="url-input"
                                label="URL"
                                variant="standard"
                                onChange={handleChangeInput}
                                value={text}
                                error={!!errors.url}
                                helperText={errors.url}
                            />
                            <FormControl error={!!errors.urlType} sx={{ width: "100%" }}>
                                <InputLabel id="urltype-select-label">URLType</InputLabel>
                                <Select
                                    sx={{ minWidth: 270, marginRight: 2, marginBottom: 2 }}
                                    labelId="urltype-select-label"
                                    id="urltype-select"
                                    value={urltype}
                                    label="URLType"
                                    onChange={handleChangeSelect}
                                >
                                    <MenuItem value={"URL"}>URL</MenuItem>
                                    <MenuItem value={"GI"}>原神</MenuItem>
                                    <MenuItem value={"HSR"}>崩壊：スターレイル</MenuItem>
                                    <MenuItem value={"ZZZ"}>ゼンレスゾーンゼロ</MenuItem>
                                </Select>
                                {errors.urlType && <Typography color="error" variant="caption">{errors.urlType}</Typography>}
                            </FormControl>
                            
                            <div>
                                <LocalizationProvider dateAdapter={AdapterDayjs} >
                                    <Stack spacing={2} sx={{ maxWidth: 270, marginRight: 2, marginBottom: 2, margintop: 2 }}>
                                        <DateTimePicker 
                                            format="YYYY/MM/DD HH:mm:ss" 
                                            value={value} 
                                            onChange={setValue} 
                                            timezone="system"
                                            slotProps={{
                                                textField: {
                                                    error: !!errors.ttl,
                                                    helperText: errors.ttl
                                                }
                                            }}
                                        />
                                    </Stack>
                                </LocalizationProvider>
                            </div>
                            
                            <Box justifyContent="flex-end" display="flex">
                                <Fab variant="extended" size="medium" color="primary" onClick={handleAdd} sx={{ marginRight: 1, marginBottom: 1 }} >
                                    <SendIcon sx={{ mr: 1 }} />
                                    追加
                                </Fab>
                            </Box>
                            <Typography align='left' sx={{ mr: 1 }} color='textSecondary'>枠外を押して閉じる</Typography>
                        </div>
                    </Typography>
                </Box>
            </Modal>
            
            {/* 編集用モーダル */}
            <Modal
                open={isEditModalOpen}
                onClose={handleCloseEditModal}
                aria-labelledby="edit-modal-title"
                aria-describedby="edit-modal-description"
            >
                <Box sx={modalStyle}>
                    <Typography id="edit-modal-title" variant="h6" component="h2">
                        情報を編集する
                    </Typography>
                    <Typography id="edit-modal-description" sx={{ mt: 2 }}>
                        <div>
                            <TextField
                                sx={{ width: "100%", maxWidth: 270, marginRight: 2, marginBottom: 2 }}
                                required
                                id="url-edit-input"
                                label="URL"
                                variant="standard"
                                onChange={handleChangeInput}
                                value={text}
                                error={!!errors.url}
                                helperText={errors.url}
                            />
                            <FormControl error={!!errors.urlType} sx={{ width: "100%" }}>
                                <InputLabel id="urltype-edit-select-label">URLType</InputLabel>
                                <Select
                                    sx={{ minWidth: 270, marginRight: 2, marginBottom: 2 }}
                                    labelId="urltype-edit-select-label"
                                    id="urltype-edit-select"
                                    value={urltype}
                                    label="URLType"
                                    onChange={handleChangeSelect}
                                >
                                    <MenuItem value={"URL"}>URL</MenuItem>
                                    <MenuItem value={"GI"}>原神</MenuItem>
                                    <MenuItem value={"HSR"}>崩壊：スターレイル</MenuItem>
                                    <MenuItem value={"ZZZ"}>ゼンレスゾーンゼロ</MenuItem>
                                </Select>
                                {errors.urlType && <Typography color="error" variant="caption">{errors.urlType}</Typography>}
                            </FormControl>
                            
                            <div>
                                <LocalizationProvider dateAdapter={AdapterDayjs} >
                                    <Stack spacing={2} sx={{ maxWidth: 270, marginRight: 2, marginBottom: 2, margintop: 2 }}>
                                        <DateTimePicker 
                                            format="YYYY/MM/DD HH:mm:ss" 
                                            value={value} 
                                            onChange={setValue} 
                                            timezone="system"
                                            slotProps={{
                                                textField: {
                                                    error: !!errors.ttl,
                                                    helperText: errors.ttl
                                                }
                                            }}
                                        />
                                    </Stack>
                                </LocalizationProvider>
                            </div>
                            
                            <Box justifyContent="flex-end" display="flex">
                                <Fab variant="extended" size="medium" color="primary" onClick={handleUpdate} sx={{ marginRight: 1, marginBottom: 1 }} >
                                    <EditIcon sx={{ mr: 1 }} />
                                    更新
                                </Fab>
                            </Box>
                            <Typography align='left' sx={{ mr: 1 }} color='textSecondary'>枠外を押して閉じる</Typography>
                        </div>
                    </Typography>
                </Box>
            </Modal>
            
            {/* 通知用スナックバー */}
            <Snackbar open={snackbar.open} autoHideDuration={6000} onClose={handleCloseSnackbar}>
                <Alert onClose={handleCloseSnackbar} severity={snackbar.severity} sx={{ width: '100%' }}>
                    {snackbar.message}
                </Alert>
            </Snackbar>
        </div>
    );
};

export default App;
