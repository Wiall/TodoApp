import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../config/api.config';
import {
  Task,
  TaskQuery,
  TaskRequest,
  UpdateTaskRequest,
  PagedTasksResponse
} from '../../models/task.models';

@Injectable({
  providedIn: 'root'
})
export class TaskService {

  private readonly apiUrl =
    `${API_BASE_URL}/api/tasks`;

  constructor(
    private readonly http: HttpClient
  ) {}

  getAll(
    query: TaskQuery = {}
  ): Observable<PagedTasksResponse> {

    let params = new HttpParams();

    if (query.search) {
      params = params.set(
        'Search',
        query.search
      );
    }

    if (query.categoryId) {
      params = params.set(
        'CategoryId',
        query.categoryId
      );
    }

    if (query.withoutCategory !== undefined) {
      params = params.set(
        'WithoutCategory',
        query.withoutCategory
      );
    }

    if (query.isDone !== undefined) {
      params = params.set(
        'IsDone',
        query.isDone
      );
    }

    if (query.sortBy) {
      params = params.set(
        'SortBy',
        query.sortBy
      );
    }

    if (query.sortDescending !== undefined) {
      params = params.set(
        'SortDescending',
        query.sortDescending
      );
    }

    if (query.page !== undefined) {
      params = params.set(
        'Page',
        query.page
      );
    }

    if (query.pageSize !== undefined) {
      params = params.set(
        'PageSize',
        query.pageSize
      );
    }

    return this.http.get<PagedTasksResponse>(
      this.apiUrl,
      { params }
    );
  }
  getById(
    id: string
  ): Observable<Task> {
    return this.http.get<Task>(
      `${this.apiUrl}/${id}`
    );
  }

  create(
    request: TaskRequest
  ): Observable<string> {
    return this.http.post(
      this.apiUrl,
      request,
      {
        responseType: 'text'
      }
    );
  }

  update(
    id: string,
    request: UpdateTaskRequest
  ): Observable<string> {
    return this.http.put(
      `${this.apiUrl}/${id}`,
      request,
      {
        responseType: 'text'
      }
    );
  }

  delete(
    id: string
  ): Observable<string> {
    return this.http.delete(
      `${this.apiUrl}/${id}`,
      {
        responseType: 'text'
      }
    );
  }
}
