import { useState } from "react";

// TodoItem�̌^�錾
type TodoItem = {
    id?: number;
    name: string;
    isComplete: boolean;
};

// �����l
const initialValues = [
    {
        id: 1,
        name: "�v���O���~���O",
        isComplete: false,
    },
    {
        id: 2,
        name: "�����j���O",
        isComplete: true,
    },
];

// Todo�R���|�[�l���g
export const Todo = () => {
    // Todo�A�C�e���I�u�W�F�N�g�̔z����Ǘ�����state
    const [todos, setTodos] = useState<TodoItem[]>(initialValues);

    return (
        <div>
            <h1>Todo���X�g</h1>
            <input type="text" />
            <button>�ǉ�</button>
            <ul>
                {/* todo�A�C�e���̔z���W�J */}
                {todos.map((todo) => (
                    <li key={todo.id}>
                        <input type="checkbox" />
                        {/* �����t���O��true�̏ꍇ�͎���������\�� */}
                        {todo.isComplete ? (
                            <span style={{ textDecorationLine: "line-through" }}>
                                {todo.name}
                            </span>
                        ) : (
                            <span>{todo.name}</span>
                        )}
                        <button>�폜</button>
                    </li>
                ))}
            </ul>
        </div>
    );
};