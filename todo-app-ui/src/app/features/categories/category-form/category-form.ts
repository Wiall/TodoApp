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
  CategoryRequest
} from '../../../models/category.models';

@Component({
  selector: 'app-category-form',
  imports: [ReactiveFormsModule],
  templateUrl: './category-form.html',
  styleUrl: './category-form.scss'
})
export class CategoryForm implements OnChanges {

  @Input()
  category: Category | null = null;

  @Output()
  saved = new EventEmitter<CategoryRequest>();

  @Output()
  cancelled = new EventEmitter<void>();

  get isEditMode(): boolean {
    return this.category !== null;
  }

  categoryForm!: FormGroup;

  constructor(private readonly formBuilder: FormBuilder) {
    this.categoryForm = this.formBuilder.nonNullable.group({
      name: ['', Validators.required],
      description: ['']
    });
  }

  ngOnChanges(
    changes: SimpleChanges
  ): void {

    if (
      changes['category'] &&
      this.category
    ) {
      this.categoryForm.patchValue({
        name: this.category.name,
        description:
          this.category.description ?? ''
      });

      return;
    }

    if (
      changes['category'] &&
      !this.category
    ) {
      this.categoryForm.reset({
        name: '',
        description: ''
      });
    }
  }

  submit(): void {
    if (this.categoryForm.invalid) {
      this.categoryForm.markAllAsTouched();
      return;
    }

    const value =
      this.categoryForm.getRawValue();

    this.saved.emit({
      name: value.name.trim(),
      description:
        value.description.trim() || null
    });
  }

  cancel(): void {
    this.cancelled.emit();
  }
}
