import {
  Component,
  EventEmitter,
  Input,
  Output
} from '@angular/core';

import {
  Task
} from '../../../models/task.models';
import {DatePipe} from '@angular/common';

@Component({
  selector: 'app-task-details',
  imports: [
    DatePipe
  ],
  templateUrl: './task-details.html',
  styleUrl: './task-details.scss'
})
export class TaskDetails {

  @Input({ required: true })
  task!: Task;

  @Output()
  closed = new EventEmitter<void>();

  @Output()
  editRequested = new EventEmitter<Task>();

  @Output()
  deleteRequested = new EventEmitter<Task>();

  formatDueDate(): string {
    if (!this.task.dueTo) {
      return 'No due date';
    }

    return new Intl.DateTimeFormat(
      'en-US',
      {
        dateStyle: 'medium',
        timeStyle: 'short'
      }
    ).format(
      new Date(this.task.dueTo)
    );
  }
}
