
import { useState, useEffect, } from "react";
import axios from "axios";
import * as React from 'react';

type UrlCodes = {
    id?: number;
    url: string;
    urlType: string;
    ttl: string;
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

export const App = () => {

    const [urlCodes, setUrlCodes] = useState<UrlCodes[]>([]);

    // テキストボックスの文字列を管理するstate
    const [text, setText] = useState("");

    // テキストボックス入力時の処理
    const handleChangeInput = (e: React.ChangeEvent<HTMLInputElement>) => {
        // テキストボックスの文字列をstateにセット
        setText(e.target.value);
    };

    const handleAdd = async () => {
        // 新しいアイテムのオブジェクトを作成（idはDB側で自動採番するため省略）
        const newCodes = { url: text, urltype: 'URL', timeLimit:'2025/03/25-00:59:59+09:00' };

        try {
            // APIにPOSTリクエストし、レスポンスから登録したTodoアイテムオブジェクトを取り出す
            const { data } = await axios.post('api/UrlDetaModels', newCodes);

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
            await axios.put(`api/UrlDetaModels/${id}`, targetCode);

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
                const { data } = await axios.get('api/UrlDetaModels');
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
        :<p>connected</p>;
        

    return (
        <div>
            <h1 id="tableLabel">HoYoTool</h1>
            <p>このコンポーネントは、サーバーからデータを取得しています。</p>
            <input type="text" onChange={handleChangeInput} value={text} />
            <button onClick={handleAdd}>追加</button>
            <ul>
                {urlCodes.map((code) => (
                    <li key={code.id}>
                        <a href={code.url}>{code.url}</a> <span>{code.urlType}</span> <span>有効期限：{code.ttl}</span>
                    </li>
                ))}
            </ul>
        </div>
    );
};

export default App;