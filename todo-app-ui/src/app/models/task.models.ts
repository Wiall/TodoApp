export interface Task {
  id: string;
  title: string;
  description: string;
  isDone: boolean;
  dueTo: string | null;
  createdAt: string;
  categoryId: string | null;
  categoryName: string | null;
}

export interface TaskQuery {
  search?: string;
  categoryId?: string;
  withoutCategory?: boolean;
  isDone?: boolean;
  sortBy?: string;
  sortDescending?: boolean;
  page?: number;
  pageSize?: number;
}

export interface TaskRequest {
  title: string;
  description: string;
  categoryId: string | null;
  dueTo: string | null;
}

export interface UpdateTaskRequest
  extends TaskRequest {
  isDone: boolean;
}

export interface PagedTasksResponse {
  items: Task[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}
