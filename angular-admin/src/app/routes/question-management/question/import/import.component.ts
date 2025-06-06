import { CoreModule, LocalizationService } from '@abp/ng.core';
import { Component, OnInit, TemplateRef, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { FooterToolbarModule } from '@delon/abc/footer-toolbar';
import { PageHeaderModule } from '@delon/abc/page-header';
import { _HttpClient } from '@delon/theme';
import { OptionService, QuestionBankService, QuestionService } from '@proxy/admin/controllers';
import { QuestionBankListDto } from '@proxy/admin/question-management/question-banks';
import { QuestionImportDto } from '@proxy/admin/question-management/questions';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { finalize, map, tap } from 'rxjs';

@Component({
  selector: 'app-question-management-question-import',
  standalone: true,
  imports: [
    CoreModule,
    PageHeaderModule,
    FooterToolbarModule,
    NzSpinModule,
    NzCardModule,
    NzGridModule,
    NzFormModule,
    NzSelectModule,
    NzInputModule,
    NzButtonModule
  ],
  templateUrl: './import.component.html'
})
export class QuestionManagementQuestionImportComponent implements OnInit {
  private fb = inject(FormBuilder);
  private modal = inject(NzModalService);
  private router = inject(Router);
  private optionService = inject(OptionService);
  private localizationService = inject(LocalizationService);
  private questionService = inject(QuestionService);
  private questionBankService = inject(QuestionBankService);

  isConfirmLoading: boolean = false;
  form: FormGroup = null;

  question: QuestionImportDto;
  questionTypes: Array<{ label: string; value: number }> = [];
  questionBanks: QuestionBankListDto[];

  ngOnInit(): void {
    this.question = {} as QuestionImportDto;
    this.buildForm();
  }
  buildForm() {
    this.questionBankService
      .getList({ skipCount: 0, maxResultCount: 100 })
      .pipe(
        tap(res => {
          this.optionService
            .getQuestionTypes()
            .pipe(
              map(res => {
                Object.keys(res).forEach(key => {
                  this.questionTypes.push({
                    label: this.localizationService.instant(`Exam::QuestionType:${key}`),
                    value: +key
                  });
                });
              })
            )
            .subscribe();
          this.questionBanks = res.items;

          this.form = this.fb.group({
            content: [this.question.content || '', [Validators.required]],
            questionType: [null, [Validators.required]],
            questionBankId: [this.question.questionBankId || '', [Validators.required]]
          });
        })
      )
      .subscribe();
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

    this.questionService
      .import({
        ...this.form.value
      })
      .pipe(
        tap(res => {
          this.goback();
        }),
        finalize(() => (this.isConfirmLoading = false))
      )
      .subscribe();
  }
  showTips(tplTitle: TemplateRef<{}>) {
    const modal: NzModalRef = this.modal.create({
      nzTitle: this.localizationService.instant('Exam:Tips'),
      nzContent: tplTitle,
      nzClosable: true,
      nzFooter: null
    });
  }
  back(e: MouseEvent) {
    e.preventDefault();
    this.goback();
  }
  goback() {
    this.router.navigateByUrl('/question-management/question');
  }
}
