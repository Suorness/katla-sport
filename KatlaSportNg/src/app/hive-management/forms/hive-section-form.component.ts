import {Component, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {HiveSectionService} from "../services/hive-section.service";
import {HiveSection} from "../models/hive-section";
import {Subscription} from "rxjs/index";

@Component({
  selector: 'app-hive-section-form',
  templateUrl: './hive-section-form.component.html',
  styleUrls: ['./hive-section-form.component.css']
})
export class HiveSectionFormComponent implements OnInit, OnDestroy {

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private hiveSectionService: HiveSectionService
  ) { }

  hiveSection: HiveSection;
  hiveId: number;
  existed = false;
  isLoaded = false;

  sub1: Subscription;

  ngOnInit() {
      this.sub1 = this.route.params
      .subscribe(params => {

        if (params['id'])
        {
          this.hiveSectionService.getHiveSection(params['id'])
            .subscribe(
              hiveSection => {
                this.hiveSection = hiveSection;
                this.existed = true;
                this.isLoaded = true;
              }
            );

        }
        else{
          this.isLoaded = true;
        }
        if (params['hiveId']){
          this.hiveId = + params['hiveId'];
        }
    });
  }

  ngOnDestroy() {
    if (this.sub1){
      this.sub1.unsubscribe();
    }
  }

  onDelete() {
    this.setStatus(this.hiveSection.id, true);
  }

  onUndelete() {
    this.setStatus(this.hiveSection.id, false);
  }

  navigateToHivesSection() {
    this.router.navigate([`/hive/${this.hiveId}/section`]);
  }

  onCancel() {
    this.navigateToHivesSection();
  }

  onSubmit() {
    if (this.existed) {
      this.hiveSectionService.updateHiveSection(this.hiveSection)
        .subscribe(
          resp => {
            this.navigateToHivesSection();
          }
        );
    }
    else {
      this.hiveSectionService.addHiveSection(this.hiveId, this.hiveSection)
        .subscribe(
          resp => {
            this.navigateToHivesSection();
          }
        );
    }
  }

  onPurge() {
    this.hiveSectionService.deleteHiveSection(this.hiveSection.id)
      .subscribe(
        resp => {
          this.navigateToHivesSection();
        }
      );
  }

  private setStatus(hiveId: number, deletedStatus: boolean){
    this.hiveSectionService.setHiveSectionStatus(hiveId, deletedStatus)
      .subscribe(
        resp => {
          this.hiveSection.isDeleted = deletedStatus;
        }
      );
  }
}
