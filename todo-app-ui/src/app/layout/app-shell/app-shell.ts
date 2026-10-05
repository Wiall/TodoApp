import {
  Component
} from '@angular/core';

import {
  Sidebar
} from '../sidebar/sidebar';

import {
  TaskList
} from '../../features/tasks/task-list/task-list';

import {
  CategorySelection
} from '../../models/category.models';

@Component({
  selector: 'app-shell',
  imports: [
    Sidebar,
    TaskList
  ],
  templateUrl: './app-shell.html',
  styleUrl: './app-shell.scss'
})
export class AppShell {

  isSidebarOpen = true;

  selectedCategory:
    CategorySelection = 'all';

  toggleSidebar(): void {
    this.isSidebarOpen =
      !this.isSidebarOpen;
  }

  closeSidebar(): void {
    this.isSidebarOpen = false;
  }

  selectCategory(
    categorySelection: CategorySelection
  ): void {
    this.selectedCategory =
      categorySelection;
  }
}
