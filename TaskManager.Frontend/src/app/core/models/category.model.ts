export interface Category {
    categoryId: string;
    name: string;
    description: string | null;
    userId: number | null;
}

export interface CategoryRequest {
    name: string;
    description: string | null;
    userId: number | null;
}

export interface CategoryListResponse {
    isSuccess: boolean;
    message: string;
    data: Category[] | null;
}

export interface CategoryResponse {
    isSuccess: boolean;
    message: string;
    data: Category | null;
}

