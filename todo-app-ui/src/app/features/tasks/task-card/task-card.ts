import {
  Component,
  EventEmitter,
  Input,
  Output
} from '@angular/core';

import {
  FormsModule
} from '@angular/forms';

import {
  Category, CategorySelection
} from '../../../models/category.models';

import {
  Task,
  UpdateTaskRequest
} from '../../../models/task.models';

@Component({
  selector: 'app-task-card',
  imports: [FormsModule],
  templateUrl: './task-card.html',
  styleUrl: './task-card.scss'
})
export class TaskCard {

  @Input({ required: true })
  task!: Task;

  @Input()
  categories: Category[] = [];

  @Input() categorySelection: CategorySelection = 'all';

  @Output()
  saveRequested =
    new EventEmitter<{
      id: string;
      request: UpdateTaskRequest;
    }>();

  @Output()
  deleteRequested =
    new EventEmitter<Task>();

  @Output()
  openRequested =
    new EventEmitter<Task>();

  isEditing = false;

  editTitle = '';
  editDescription = '';
  editCategoryId: string | null = null;
  editDueTo: string | null = null;

  startEditing(): void {
    this.isEditing = true;

    this.editTitle = this.task.title;
    this.editDescription =
      this.task.description;

    this.editCategoryId =
      this.task.categoryId;

    this.editDueTo =
      this.task.dueTo
        ? this.task.dueTo.slice(0, 16)
        : null;
  }

  cancelEditing(): void {
    this.isEditing = false;
  }

  saveEditing(): void {
    const title =
      this.editTitle.trim();

    if (!title) {
      return;
    }

    this.saveRequested.emit({
      id: this.task.id,

      request: {
        title,
        description:
          this.editDescription.trim(),
        categoryId:
        this.editCategoryId,
        dueTo:
          this.editDueTo || null,
        isDone:
        this.task.isDone
      }
    });

    this.isEditing = false;
  }

  toggleDone(): void {
    this.saveRequested.emit({
      id: this.task.id,

      request: {
        title: this.task.title,
        description:
        this.task.description,
        categoryId:
        this.task.categoryId,
        dueTo:
        this.task.dueTo,
        isDone:
          !this.task.isDone
      }
    });
  }

  openTask(): void {
    this.openRequested.emit(
      this.task
    );
  }

  deleteTask(): void {
    this.deleteRequested.emit(this.task);
  }

  formatDueDate(): string {
    if (!this.task.dueTo) {
      return 'No due date';
    }

    return new Intl.DateTimeFormat(
      'en-US',
      {
        month: 'short',
        day: 'numeric'
      }
    ).format(
      new Date(this.task.dueTo)
    );
  }
}
