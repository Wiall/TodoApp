import {
  ChangeDetectorRef,
  Component,
  Input,
  OnChanges,
  OnInit,
  OnDestroy,
  SimpleChanges
} from '@angular/core';
import { FormsModule } from '@angular/forms';

import {
  Subject,
  debounceTime,
  distinctUntilChanged
} from 'rxjs';

import { TaskCard } from '../task-card/task-card';
import { TaskDetails } from '../task-details/task-details';
import { TaskForm, TaskFormValue } from '../task-form/task-form';

import { ConfirmDialog } from '../../../shared/components/confirm-dialog/confirm-dialog';

import { TaskService } from '../../../core/services/task.service';
import { CategoryService } from '../../../core/services/category.service';

import {
  Category,
  CategorySelection
} from '../../../models/category.models';

import {
  Task,
  UpdateTaskRequest
} from '../../../models/task.models';

@Component({
  selector: 'app-task-list',
  imports: [
    FormsModule,
    TaskCard,
    TaskDetails,
    TaskForm,
    ConfirmDialog
  ],
  templateUrl: './task-list.html',
  styleUrl: './task-list.scss'
})
export class TaskList
  implements OnInit, OnChanges, OnDestroy {

  @Input()
  categorySelection: CategorySelection =
    'all';

  categories: Category[] = [];

  tasks: Task[] = [];

  selectedTask: Task | null = null;

  editingTask: Task | null = null;

  deletingTask: Task | null = null;

  showTaskForm = false;

  isLoading = false;
  isSaving = false;

  errorMessage = '';

  searchTerm = '';
  private readonly searchSubject = new Subject<string>();

  selectedFilter:
    TaskStatusFilter = 'all';

  sortBy:
    TaskSortField = 'createdat';

  sortDescending = true;

  currentPage = 1;
  pageSize = 10;

  totalCount = 0;
  totalPages = 0;

  readonly pageSizes = [
    10,
    20,
    50
  ];

  private loadRequestId = 0;

  constructor(
    private readonly taskService: TaskService,
    private readonly categoryService:
    CategoryService,
    private readonly changeDetectorRef:
    ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadCategories();

    this.searchSubject
      .pipe(
        debounceTime(350),
        distinctUntilChanged()
      )
      .subscribe(search => {
        this.searchTerm = search;
        this.currentPage = 1;
        this.loadTasks();
      });

    this.loadTasks();
  }

  ngOnChanges(
    changes: SimpleChanges
  ): void {

    if (
      changes['categorySelection'] &&
      !changes['categorySelection'].firstChange
    ) {
      this.currentPage = 1;
      this.loadTasks();
    }
  }

  ngOnDestroy(): void {
    this.searchSubject.complete();
  }

  onSearchChange(
    value: string
  ): void {
    this.searchSubject.next(value.trim());
  }

  loadCategories(): void {
    this.categoryService
      .getAll()
      .subscribe({
        next: categories => {
          this.categories = categories;

          this.changeDetectorRef
            .markForCheck();
        },

        error: error => {
          console.error(
            'Failed to load categories',
            error
          );
        }
      });
  }

  loadTasks(): void {
    const requestId = ++this.loadRequestId;

    this.isLoading = true;
    this.errorMessage = '';

    const categoryId =
      this.categorySelection !== 'all' &&
      this.categorySelection !== 'none'
        ? this.categorySelection
        : undefined;

    const withoutCategory =
      this.categorySelection === 'none'
        ? true
        : undefined;

    const isDone =
      this.selectedFilter === 'all'
        ? undefined
        : this.selectedFilter === 'completed';

    this.taskService
      .getAll({
        search:
          this.searchTerm || undefined,

        categoryId,

        withoutCategory,

        isDone,

        sortBy:
        this.sortBy,

        sortDescending:
        this.sortDescending,

        page:
        this.currentPage,

        pageSize:
        this.pageSize
      })
      .subscribe({
        next: response => {

          if (
            requestId !== this.loadRequestId
          ) {
            return;
          }

          if (
            response.totalPages > 0 &&
            this.currentPage > response.totalPages
          ) {
            this.currentPage =
              response.totalPages;

            this.loadTasks();

            return;
          }

          this.tasks =
            response.items;

          this.currentPage =
            response.page;

          this.pageSize =
            response.pageSize;

          this.totalCount =
            response.totalCount;

          this.totalPages =
            response.totalPages;

          this.isLoading = false;

          this.changeDetectorRef
            .markForCheck();
        },

        error: error => {

          if (
            requestId !== this.loadRequestId
          ) {
            return;
          }

          console.error(
            'Failed to load tasks',
            error
          );

          this.errorMessage =
            error?.error?.message ??
            'Failed to load tasks.';

          this.isLoading = false;

          this.changeDetectorRef
            .markForCheck();
        }
      });
  }

  openCreateTaskForm(): void {
    this.editingTask = null;
    this.showTaskForm = true;
  }

  openEditTaskForm(
    task: Task
  ): void {
    this.selectedTask = null;
    this.editingTask = task;
    this.showTaskForm = true;
  }

  closeTaskForm(): void {
    this.showTaskForm = false;
    this.editingTask = null;
  }

  saveTask(
    value: TaskFormValue
  ): void {

    this.isSaving = true;

    if (this.editingTask) {

      const request:
        UpdateTaskRequest = {
        title: value.title,
        description:
        value.description,
        categoryId:
        value.categoryId,
        dueTo:
        value.dueTo,
        isDone:
        value.isDone
      };

      this.taskService
        .update(
          this.editingTask.id,
          request
        )
        .subscribe({
          next: () => {
            this.isSaving = false;

            this.closeTaskForm();
            this.loadTasks();
          },

          error: error => {
            console.error(
              'Failed to update task',
              error
            );

            this.errorMessage =
              error?.error?.message ??
              'Failed to update task.';

            this.isSaving = false;

            this.changeDetectorRef
              .markForCheck();
          }
        });

      return;
    }

    this.taskService
      .create({
        title: value.title,
        description: value.description,
        categoryId: value.categoryId,
        dueTo: value.dueTo
      })
      .subscribe({
        next: () => {

          this.isSaving = false;

          this.closeTaskForm();

          this.currentPage = 1;

          this.loadTasks();
        },

        error: error => {

          console.error(
            'Failed to create task',
            error
          );

          this.errorMessage =
            error?.error?.message ??
            'Failed to create task.';

          this.isSaving = false;

          this.changeDetectorRef
            .markForCheck();
        }
      });
  }

  saveInlineTask(
    event: {
      id: string;
      request: UpdateTaskRequest;
    }
  ): void {

    this.taskService
      .update(
        event.id,
        event.request
      )
      .subscribe({
        next: () => {
          this.loadTasks();
        },

        error: error => {
          console.error(
            'Failed to update task',
            error
          );

          this.errorMessage =
            error?.error?.message ??
            'Failed to update task.';

          this.changeDetectorRef
            .markForCheck();
        }
      });
  }

  openTask(
    task: Task
  ): void {
    this.selectedTask = task;
  }

  closeTaskDetails(): void {
    this.selectedTask = null;
  }

  requestDelete(
    task: Task
  ): void {
    this.selectedTask = null;
    this.deletingTask = task;
  }

  cancelDelete(): void {
    this.deletingTask = null;
  }

  confirmDelete(): void {
    if (!this.deletingTask) {
      return;
    }

    const taskId =
      this.deletingTask.id;

    this.deletingTask = null;

    this.isSaving = true;

    this.taskService
      .delete(taskId)
      .subscribe({
        next: () => {

          this.isSaving = false;

          this.loadTasks();
        },

        error: error => {

          console.error(
            'Failed to delete task',
            error
          );

          this.errorMessage =
            error?.error?.message ??
            'Failed to delete task.';

          this.isSaving = false;

          this.changeDetectorRef
            .markForCheck();
        }
      });
  }

  setFilter(
    filter: TaskStatusFilter
  ): void {

    this.selectedFilter = filter;
    this.currentPage = 1;

    this.loadTasks();
  }

  onSortChange(
    sortBy: string
  ): void {

    this.sortBy =
      sortBy as TaskSortField;

    this.sortDescending =
      this.defaultSortDirection(
        this.sortBy
      );

    this.currentPage = 1;

    this.loadTasks();
  }

  toggleSortDirection(): void {
    this.sortDescending =
      !this.sortDescending;

    this.currentPage = 1;

    this.loadTasks();
  }

  goToPage(page: number): void {

    if (
      page < 1 ||
      page > this.totalPages ||
      page === this.currentPage
    ) {
      return;
    }

    this.currentPage = page;
    this.loadTasks();
  }

  changePageSize(
    newPageSize: number
  ): void {

    if (
      !this.pageSizes.includes(
        newPageSize
      )
    ) {
      return;
    }

    if (
      newPageSize === this.pageSize
    ) {
      return;
    }

    this.pageSize =
      newPageSize;

    this.currentPage = 1;

    this.loadTasks();
  }

  private defaultSortDirection(
    sortBy: TaskSortField
  ): boolean {

    switch (sortBy) {

      case 'title':
        return false;

      case 'dueto':
        return false;

      case 'isdone':
        return false;

      case 'createdat':
      default:
        return true;
    }
  }

  get paginationPages(): number[] {
    const pages: number[] = [];

    const start = Math.max(
      1,
      this.currentPage - 2
    );

    const end = Math.min(
      this.totalPages,
      this.currentPage + 2
    );

    for (
      let page = start;
      page <= end;
      page++
    ) {
      pages.push(page);
    }

    return pages;
  }

  get pageTitle(): string {

    if (
      this.categorySelection === 'all'
    ) {
      return 'All tasks';
    }

    if (
      this.categorySelection === 'none'
    ) {
      return 'No category';
    }

    const category =
      this.categories.find(
        item =>
          item.id ===
          this.categorySelection
      );

    return category?.name ?? 'Tasks';
  }
}

type TaskStatusFilter =
  | 'all'
  | 'active'
  | 'completed';

type TaskSortField =
  | 'createdat'
  | 'title'
  | 'dueto'
  | 'isdone';
