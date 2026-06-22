export interface User {
    userId: number;
    firstName: string;
    lastName: string | null;
    email: string;
    password: string;
}

export interface UserRequest {
    firstName: string;
    lastName: string | null;
    email: string;
    password: string;
}

export interface UserResponse {
    isSuccess: boolean;
    message: string;
    data: User | null;
}

export interface UserListResponse {
    isSuccess: boolean;
    message: string;
    data: User[] | null;
}