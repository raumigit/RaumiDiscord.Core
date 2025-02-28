import { useState } from "react";

// TodoItemの型宣言
type TodoItem = {
    id?: number;
    name: string;
    isComplete: boolean;
};

// 初期値
const initialValues = [
    {
        id: 1,
        name: "プログラミング",
        isComplete: false,
    },
    {
        id: 2,
        name: "ランニング",
        isComplete: true,
    },
];

// UrlListコンポーネント
export const Todo = () => {
    // UrlListアイテムオブジェクトの配列を管理するstate
    const [todos] = useState<TodoItem[]>(initialValues);

    return (
        <div>
            <h1>Todoリスト</h1>
            <input type="text" />
            <button>追加</button>
            <ul>
                {/* UrlListアイテムの配列を展開 */}
                {todos.map((todo) => (
                    <li key={todo.id}>
                        <input type="checkbox" />
                        {/* 完了フラグがtrueの場合は取り消し線を表示 */}
                        {todo.isComplete ? (
                            <span style={{ textDecorationLine: "line-through" }}>
                                {todo.name}
                            </span>
                        ) : (
                            <span>{todo.name}</span>
                        )}
                        <button>削除</button>
                    </li>
                ))}
            </ul>
        </div>
    );
};