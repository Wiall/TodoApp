import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  SimpleChanges
} from '@angular/core';

import {
  FormBuilder, FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import {
  Category,
  CategorySelection
} from '../../../models/category.models';

import {
  Task
} from '../../../models/task.models';

export interface TaskFormValue {
  title: string;
  description: string;
  categoryId: string | null;
  dueTo: string | null;
  isDone: boolean;
}

@Component({
  selector: 'app-task-form',
  imports: [ReactiveFormsModule],
  templateUrl: './task-form.html',
  styleUrl: './task-form.scss'
})
export class TaskForm implements OnChanges {

  @Input()
  task: Task | null = null;

  @Input()
  categories: Category[] = [];

  @Input()
  categorySelection: CategorySelection = 'all';

  @Output()
  saved = new EventEmitter<TaskFormValue>();

  @Output()
  cancelled = new EventEmitter<void>();

  taskForm!: FormGroup;

  constructor(private readonly formBuilder: FormBuilder) {
    this.taskForm = this.formBuilder.group({
      title: this.formBuilder.nonNullable.control(
        '',
        [Validators.required, Validators.maxLength(200)]
      ),
      description: this.formBuilder.nonNullable.control(
        '',
        Validators.maxLength(2000)
      ),
      categoryId: this.formBuilder.control<string | null>(null),
      dueTo: this.formBuilder.control<string | null>(null),
      isDone: this.formBuilder.nonNullable.control(false)
    });
  }

  get isEditMode(): boolean {
    return this.task !== null;
  }

  ngOnChanges(
    changes: SimpleChanges
  ): void {

    if (
      changes['task']
    ) {
      this.populateForm();
    }
  }

  submit(): void {
    if (this.taskForm.invalid) {
      this.taskForm.markAllAsTouched();
      return;
    }

    const value =
      this.taskForm.getRawValue();

    this.saved.emit({
      title: value.title.trim(),
      description: value.description.trim(),
      categoryId: value.categoryId,
      dueTo: value.dueTo || null,
      isDone: this.task?.isDone ?? false
    });
  }

  cancel(): void {
    this.cancelled.emit();
  }

  get canChooseCategory(): boolean {
    return (
      this.task !== null ||
      this.categorySelection === 'all'
    );
  }

  get initialCategoryId(): string | null {
    if (this.task) {
      return this.task.categoryId;
    }

    if (
      this.categorySelection === 'none' ||
      this.categorySelection === 'all'
    ) {
      return null;
    }

    return this.categorySelection;
  }

  private populateForm(): void {
    if (!this.task) {
      this.taskForm.reset({
        title: '',
        description: '',
        categoryId: this.initialCategoryId,
        dueTo: null
      });

      return;
    }

    this.taskForm.patchValue({
      title: this.task.title,
      description: this.task.description,
      categoryId: this.task.categoryId,
      dueTo: this.task.dueTo
        ? this.task.dueTo.slice(0, 16)
        : null
    });
  }
}
