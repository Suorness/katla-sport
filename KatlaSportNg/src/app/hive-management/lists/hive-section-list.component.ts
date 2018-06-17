import {Component, OnDestroy, OnInit} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HiveSectionListItem } from '../models/hive-section-list-item';
import { HiveService } from '../services/hive.service';
import {Subscription} from "rxjs/index";
import {HiveSectionService} from "../services/hive-section.service";

@Component({
  selector: 'app-hive-section-list',
  templateUrl: './hive-section-list.component.html',
  styleUrls: ['./hive-section-list.component.css']
})
export class HiveSectionListComponent implements OnInit, OnDestroy {

  hiveId: number;
  hiveSections: Array<HiveSectionListItem>;
  sub1: Subscription;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private hiveService: HiveService,
    private hiveSectionService: HiveSectionService
  ) { }

  ngOnInit() {
    this.sub1 = this.route.params
      .subscribe(p => {
        this.hiveId = p['id'];
        this.hiveService.getHiveSections(this.hiveId)
          .subscribe(s => this.hiveSections = s);
    })
  }

  ngOnDestroy() {
    if (this.sub1){
      this.sub1.unsubscribe();
    }
  }

  onDelete(hiveSectionId: number) {
    this.setStatus(hiveSectionId, true);
  }

  onUndelete(hiveSectionId: number) {
    this.setStatus(hiveSectionId, false);
  }

  private setStatus(hiveSectionId: number, status: boolean) {
    var hiveSection = this.hiveSections
      .find(h => h.id == hiveSectionId);
    this.hiveSectionService
      .setHiveSectionStatus(hiveSectionId,status)
      .subscribe(c => hiveSection.isDeleted = status);
  }
}
