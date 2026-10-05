import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output
} from '@angular/core';

import { Router } from '@angular/router';

import { CategoryService } from '../../core/services/category.service';
import {
  Category,
  CategoryRequest,
  CategorySelection
} from '../../models/category.models';

import { CategoryForm } from '../../features/categories/category-form/category-form';
import { AuthService } from '../../core/services/auth.service';
import {ConfirmDialog} from '../../shared/components/confirm-dialog/confirm-dialog';

@Component({
  selector: 'app-sidebar',
  imports: [CategoryForm, ConfirmDialog],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss'
})
export class Sidebar implements OnInit {
  @Input()
  isOpen = true;

  @Output()
  toggleRequested = new EventEmitter<void>();

  @Output()
  categorySelected =
    new EventEmitter<CategorySelection>();

  categories: Category[] = [];

  selectedCategoryId = '';

  activeMenuId: string | null = null;

  isLoading = false;
  errorMessage = '';

  showCategoryForm = false;
  editingCategory: Category | null = null;
  deletingCategory: Category | null = null;

  constructor(
    private readonly categoryService: CategoryService,
    private readonly changeDetectorRef: ChangeDetectorRef,
    private readonly router: Router,
    private readonly authService: AuthService,
  ) {}

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.categoryService.getAll().subscribe({
      next: categories => {
        this.categories = categories;

        if (
          !this.selectedCategoryId &&
          categories.length > 0
        ) {
          this.selectedCategoryId =
            categories[0].id;

          this.categorySelected.emit(
            categories[0].id
          );
        }

        this.isLoading = false;

        this.changeDetectorRef.markForCheck();
      },

      error: error => {
        console.error(
          'Failed to load categories',
          error
        );

        this.errorMessage =
          'Failed to load categories.';

        this.isLoading = false;

        this.changeDetectorRef.markForCheck();
      }
    });
  }

  selectCategory(
    categoryId: string
  ): void {
    this.selectedCategoryId =
      categoryId;

    this.activeMenuId = null;

    this.categorySelected.emit(
      categoryId
    );

    if (
      window.matchMedia(
        '(max-width: 767.98px)'
      ).matches
    ) {
      this.toggleRequested.emit();
    }
  }

  selectAllTasks(): void {
    this.selectedCategoryId = '__all__';
    this.activeMenuId = null;

    this.categorySelected.emit('all');

    if (
      window.matchMedia(
        '(max-width: 767.98px)'
      ).matches
    ) {
      this.toggleRequested.emit();
    }
  }

  selectNoCategory(): void {
    this.selectedCategoryId = '__none__';
    this.activeMenuId = null;

    this.categorySelected.emit('none');

    if (
      window.matchMedia(
        '(max-width: 767.98px)'
      ).matches
    ) {
      this.toggleRequested.emit();
    }
  }

  toggleCategoryMenu(categoryId: string): void {
    this.activeMenuId =
      this.activeMenuId === categoryId
        ? null
        : categoryId;
  }

  createOrUpdateCategory(
    request: CategoryRequest
  ): void {

    if (this.editingCategory) {

      this.categoryService
        .update(
          this.editingCategory.id,
          request
        )
        .subscribe({
          next: updatedCategory => {

            this.categories =
              this.categories.map(category =>
                category.id === updatedCategory.id
                  ? updatedCategory
                  : category
              );

            this.closeCategoryForm();

            this.changeDetectorRef.markForCheck();
          },

          error: error => {
            console.error(
              'Failed to update category',
              error
            );

            this.errorMessage =
              error?.error?.message ??
              'Failed to update category.';

            this.changeDetectorRef.markForCheck();
          }
        });

      return;
    }

    this.categoryService
      .create(request)
      .subscribe({
        next: createdCategory => {

          this.categories = [
            ...this.categories,
            createdCategory
          ];

          this.selectedCategoryId = createdCategory.id;

          this.categorySelected.emit(
            createdCategory.id
          );

          this.closeCategoryForm();

          this.changeDetectorRef.markForCheck();
        },

        error: error => {
          console.error(
            'Failed to create category',
            error
          );

          this.errorMessage =
            error?.error?.message ??
            'Failed to create category.';

          this.changeDetectorRef.markForCheck();
        }
      });
  }

  requestDeleteCategory(
    category: Category
  ): void {
    this.activeMenuId = null;
    this.deletingCategory = category;
  }

  cancelDeleteCategory(): void {
    this.deletingCategory = null;
  }

  confirmDeleteCategory(): void {
    if (!this.deletingCategory) {
      return;
    }

    const categoryId =
      this.deletingCategory.id;

    this.deletingCategory = null;

    this.categoryService
      .delete(categoryId)
      .subscribe({
        next: () => {

          this.categories =
            this.categories.filter(
              category =>
                category.id !== categoryId
            );

          if (
            this.selectedCategoryId ===
            categoryId
          ) {
            this.selectedCategoryId =
              this.categories[0]?.id ?? '';

            this.categorySelected.emit(
              this.selectedCategoryId || 'all'
            );
          }

          this.changeDetectorRef
            .markForCheck();
        },

        error: error => {
          console.error(
            'Failed to delete category',
            error
          );

          this.errorMessage =
            error?.error?.message ??
            'Failed to delete category.';

          this.changeDetectorRef
            .markForCheck();
        }
      });
  }

  openCreateCategoryForm(): void {
    this.editingCategory = null;
    this.showCategoryForm = true;
    this.activeMenuId = null;
  }

  openEditCategoryForm(
    category: Category
  ): void {
    this.editingCategory = category;
    this.showCategoryForm = true;
    this.activeMenuId = null;
  }

  closeCategoryForm(): void {
    this.showCategoryForm = false;
    this.editingCategory = null;
  }

  logout(): void {
    this.authService.logout().subscribe({
      next: () => {
        this.router.navigate(['/login']);
      },

      error: error => {
        console.error(
          'Logout failed',
          error
        );

        this.authService.clearSession();

        this.router.navigate(['/login']);
      }
    });
  }
}
