import { LocalizationService } from '@abp/ng.core';
import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { finalize, tap } from 'rxjs/operators';
import { NzModalRef } from 'ng-zorro-antd/modal';
import { GetExamingForEditorOutput } from '@proxy/super-abp/exam/admin/exam-management/exams';
import { ExamingService } from '@proxy/super-abp/exam/admin/controllers';
import { ActivatedRoute, Router } from '@angular/router';
import { dateTimePickerUtil } from '@delon/util';
import { DisabledTimeFn, DisabledTimePartial } from 'ng-zorro-antd/date-picker';

@Component({
  selector: 'app-exam-management-examing-edit',
  templateUrl: './edit.component.html',
  styles: [
    `
      .ant-form-item-label {
        width: 95px;
      }
    `
  ]
})
export class ExamManagementExamingEditComponent implements OnInit {
  @Input()
  examingId: string;
  examing: GetExamingForEditorOutput;

  loading = false;
  isConfirmLoading = false;
  showExamingTime: false;
  form: FormGroup = null;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private messageService: NzMessageService,
    private localizationService: LocalizationService,
    private examingService: ExamingService
  ) {}

  get isLimitedTime() {
    return this.form.get('isLimitedTime');
  }
  get examingTimes() {
    return this.form.get('examingTimes');
  }
  get startTime() {
    return this.form.get('startTime');
  }
  get endTime() {
    return this.form.get('endTime');
  }
  range(start: number, end: number): number[] {
    const result: number[] = [];
    for (let i = start; i < end; i++) {
      result.push(i);
    }
    return result;
  }
  disabledDate = (current: Date): boolean => dateTimePickerUtil.getDiffDays(current, new Date()) <= 0;

  ngOnInit(): void {
    this.loading = true;
    if (this.examingId) {
      this.examingService
        .getEditor(this.examingId)
        .pipe(
          tap(response => {
            this.examing = response;
            this.buildForm();
            this.loading = false;
          })
        )
        .subscribe();
    } else {
      this.examing = {} as GetExamingForEditorOutput;
      this.buildForm();
      this.loading = false;
    }
  }

  buildForm() {
    this.form = this.fb.group({
      name: [this.examing.name || '', [Validators.required]],
      description: [this.examing.description || ''],
      score: [this.examing.score || 0],
      passingScore: [this.examing.passingScore || 0],
      totalTime: [this.examing.totalTime || 0],
      isLimitedTime: [false],
      examingTimes: [[]],
      startTime: [this.examing.startTime || new Date()],
      endTime: [this.examing.endTime || new Date()],
      repositories: this.fb.array([])
    });
    if (this.examing.startTime && this.examing.endTime) {
      this.isLimitedTime.setValue(true);
      this.examingTimes.setValue([this.examing.startTime, this.examing.endTime]);
    }
  }

  save() {
    if (!this.form.valid || this.isConfirmLoading) {
      for (const key of Object.keys(this.form.controls)) {
        this.form.controls[key].markAsDirty();
        this.form.controls[key].updateValueAndValidity();
      }
      return;
    }
    this.isConfirmLoading = true;

    if (this.examingId) {
      this.examingService
        .update(this.examingId, {
          ...this.examing,
          ...this.form.value
        })
        .pipe(
          tap(response => {
            this.messageService.success(this.localizationService.instant('Exam::SaveSuccessfully'));
            this.goback();
          }),
          finalize(() => (this.isConfirmLoading = false))
        )
        .subscribe();
    } else {
      this.examingService
        .create({
          ...this.form.value
        })
        .pipe(
          tap(response => {
            this.messageService.success(this.localizationService.instant('*::SaveSucceed'));
            this.goback();
          }),
          finalize(() => (this.isConfirmLoading = false))
        )
        .subscribe();
    }
  }

  examingTimeChange(e) {
    this.startTime.setValue(e[0]);
    this.endTime.setValue(e[1]);
  }
  changeExamingTimeStatus(e) {
    this.showExamingTime = e;
  }
  back(e: MouseEvent) {
    e.preventDefault();
    this.goback();
  }
  goback() {
    this.router.navigateByUrl('/exam-management/examing');
  }
}