import { ConfigStateService, CoreModule, LocalizationService, PermissionService } from '@abp/ng.core';
import { Component, OnInit, ViewChild, inject } from '@angular/core';
import { Router } from '@angular/router';
import { PageHeaderModule } from '@delon/abc/page-header';
import { STChange, STColumn, STComponent, STModule, STPage } from '@delon/abc/st';
import { DelonFormModule, SFSchema, SFStringWidgetSchema } from '@delon/form';
import { ModalHelper } from '@delon/theme';
import { ExaminationService } from '@proxy/admin/controllers';
import { ExamListDto, GetExamsInput } from '@proxy/admin/exam-management/exams';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzMessageService } from 'ng-zorro-antd/message';
import { tap } from 'rxjs/operators';

import { ExamManagementExamEditComponent } from './edit/edit.component';

@Component({
  selector: 'app-exam-management-exam',
  templateUrl: './exam.component.html',
  standalone: true,
  imports: [CoreModule, PageHeaderModule, DelonFormModule, STModule, NzCardModule, NzButtonModule]
})
export class ExamManagementExamComponent implements OnInit {
  private modal = inject(ModalHelper);
  private router = inject(Router);
  private localizationService = inject(LocalizationService);
  private messageService = inject(NzMessageService);
  private permissionService = inject(PermissionService);
  private examService = inject(ExaminationService);
  exams: ExamListDto[];
  total: number;
  loading = false;
  params: GetExamsInput;
  page: STPage = {
    show: true,
    showSize: true,
    front: false,
    pageSizes: [10, 20, 30, 40, 50]
  };
  searchSchema: SFSchema = {
    properties: {
      name: {
        type: 'string',
        title: '',
        ui: {
          width: 250,
          placeholder: this.localizationService.instant('Exam::Placeholder', this.localizationService.instant('Exam::Name'))
        } as SFStringWidgetSchema
      }
    }
  };
  @ViewChild('st', { static: false }) st: STComponent;
  columns: STColumn[] = [
    { title: this.localizationService.instant('Exam::Name'), index: 'name' },
    { title: this.localizationService.instant('Exam::Score'), index: 'score', render: 'score' },
    {
      title: {
        text: this.localizationService.instant('Exam::TotalTime'),
        optional: `（${this.localizationService.instant('Exam::Unit')}：${this.localizationService.instant('Exam::Minute')}）`
      },
      index: 'totalTime'
    },
    { title: this.localizationService.instant('Exam::Status'), render: 'status' },
    { title: this.localizationService.instant('Exam::StartTime'), index: 'startTime' },
    { title: this.localizationService.instant('Exam::EndTime'), index: 'endTime' },
    {
      title: this.localizationService.instant('Exam::Actions'),
      buttons: [
        {
          icon: 'edit',
          type: 'modal',
          tooltip: this.localizationService.instant('Exam::Edit'),
          iif: record => {
            return this.permissionService.getGrantedPolicy('Exam.Exam.Update') && record.status === 0;
          },
          modal: {
            component: ExamManagementExamEditComponent,
            params: (record: any) => ({
              examId: record.id
            })
          },
          click: 'reload'
        },
        {
          icon: 'delete',
          type: 'del',
          tooltip: this.localizationService.instant('Exam::Delete'),
          pop: {
            title: this.localizationService.instant('Exam::AreYouSure'),
            okType: 'danger',
            icon: 'star'
          },
          iif: record => {
            return this.permissionService.getGrantedPolicy('Exam.Exam.Delete') && record.status === 0;
          },
          click: (record, _modal, component) => {
            this.examService.delete(record.id).subscribe(response => {
              this.messageService.success(this.localizationService.instant('Exam::DeletedSuccessfully', record.name));
              // tslint:disable-next-line: no-non-null-assertion
              component!.removeRow(record);
            });
          }
        },
        {
          text: this.localizationService.instant('Exam::More'),
          children: [
            {
              iif: record => {
                return record.status === 2;
              },
              text: this.localizationService.instant('Exam::ExamRecord'),
              modal: {
                component: ExamManagementExamEditComponent,
                params: (record: any) => ({
                  examId: record.id
                })
              },
              click: record => {
                this.router.navigateByUrl(`/exam-management/user-exam-user?examId=${record.id}`);
              }
            },
            {
              type: 'divider'
            },
            {
              text: this.localizationService.instant('Exam::Publish'),
              iif: record => {
                return this.permissionService.getGrantedPolicy('Exam.Exam.Publish') && record.status === 0;
              },
              click: (record, _modal, component) => {
                this.examService.publish(record.id).subscribe(response => {
                  this.st.reload();
                });
              }
            },
            {
              text: this.localizationService.instant('Exam::Cancel'),
              iif: record => {
                return this.permissionService.getGrantedPolicy('Exam.Exam.Cancel') && record.status !== 4 && record.status !== 0;
              },
              click: (record, _modal, component) => {
                this.examService.cancel(record.id).subscribe(response => {
                  this.st.reload();
                });
              }
            }
          ]
        }
      ]
    }
  ];

  ngOnInit() {
    this.params = this.resetParameters();
    this.getList();
  }
  getList() {
    this.loading = true;
    this.examService
      .getList(this.params)
      .pipe(tap(() => (this.loading = false)))
      .subscribe(response => ((this.exams = response.items), (this.total = response.totalCount)));
  }
  resetParameters(): GetExamsInput {
    return {
      skipCount: 0,
      maxResultCount: 10
    };
  }
  change(e: STChange) {
    if (e.type === 'pi' || e.type === 'ps') {
      this.params.skipCount = (e.pi - 1) * e.ps;
      this.params.maxResultCount = e.ps;
      this.getList();
    } else if (e.type === 'sort') {
      this.params.sorting = `${e.sort?.column?.index as string} ${e.sort.value === 'ascend' ? 'asc' : 'desc'}`;
      this.getList();
    }
  }
  reset() {
    this.params = this.resetParameters();
    this.st.load(1);
  }
  search(e) {
    if (e.name) {
      this.params.name = e.name;
    } else {
      delete this.params.name;
    }
    this.st.load(1);
  }
  add() {
    this.modal.createStatic(ExamManagementExamEditComponent, { examId: '' }).subscribe(() => this.st.reload());
  }
}
