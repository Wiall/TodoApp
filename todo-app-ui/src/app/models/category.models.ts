export interface Category {
  id: string;
  name: string;
  description: string | null;
}

export interface CategoryRequest {
  name: string;
  description: string | null;
}

export type CategorySelection =
  | 'all'
  | 'none'
  | string;
