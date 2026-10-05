import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { API_BASE_URL } from '../config/api.config';

import {
  Category,
  CategoryRequest
} from '../../models/category.models';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {

  private readonly apiUrl =
    `${API_BASE_URL}/api/categories`;

  constructor(
    private readonly http: HttpClient
  ) {}

  getAll(): Observable<Category[]> {
    return this.http.get<Category[]>(
      this.apiUrl
    );
  }

  getById(
    id: string
  ): Observable<Category> {
    return this.http.get<Category>(
      `${this.apiUrl}/${id}`
    );
  }

  create(
    request: CategoryRequest
  ): Observable<Category> {
    return this.http.post<Category>(
      this.apiUrl,
      request
    );
  }

  update(
    id: string,
    request: CategoryRequest
  ): Observable<Category> {
    return this.http.put<Category>(
      `${this.apiUrl}/${id}`,
      request
    );
  }

  delete(
    id: string
  ): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/${id}`
    );
  }
}
