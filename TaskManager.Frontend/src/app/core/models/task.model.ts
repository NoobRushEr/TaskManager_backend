export interface Task {
    taskId: string;
    title: string;
    description: string | null;
    createdAt: Date;
    completedAt: Date | null;
    dueDate: Date | null;
    priority: string | null;
    status: string | null;
    categoryId: string | null;
    isDeleted: boolean;
    deletedAt: Date | null;
}


export interface TaskRequest {
    title: string;
    description: string | null;
    dueDate: Date | null;
    priority: string | null;
    status: string | null;
    categoryId: string | null;
}

export interface TaskUpdateRequest {
    taskId: string;
    title: string | null;
    description: string | null;
    dueDate: Date | null;
    priority: string | null;
    status: string | null;
    categoryId: string | null;
}

export interface TaskListResponse {
    isSuccess: boolean;
    message: string;
    data: Task[] | null;
}

export interface TaskResponse {
    isSuccess: boolean;
    message: string;
    data: Task | null;
}

