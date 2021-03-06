﻿using SterreFenna.Business.Projects.Queries;
using SterreFenna.Business.Series.Commands;
using SterreFenna.Business.Series.Queries;
using SterreFenna.WebPresentation.Areas.Admin.Models;
using SterreFenna.WebPresentation.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SterreFenna.WebPresentation.Areas.Admin.Controllers
{
    [Authorize]
    public class SerieController : Controller
    {
        private readonly CreateSerieCommand _createSerieCommand;
        private readonly EditSerieCommand _editSerieCommand;
        private readonly GetProjectListOverviewQuery _getProjectListOverviewQuery;

        public SerieController(
            CreateSerieCommand createGalleryCommand,
            EditSerieCommand editSerieCommand,
            GetProjectListOverviewQuery getProjectListOverviewQuery)
        {
            _createSerieCommand = createGalleryCommand;
            _editSerieCommand = editSerieCommand;
            _getProjectListOverviewQuery = getProjectListOverviewQuery;
        }

        public ActionResult Edit(int id)
        {
            var model = new EditSerieModel
            {
                SerieDetails = new GetSerieByIdCommand().Handle(id),
                Projects = new GetProjectOverviewQuery().Handle(),
            };

            return View(model);
        }

        public ActionResult SubmitEdit(PostedNewSerieModel model)
        {
            var serie = new GetSerieByIdCommand().Handle(model.SerieId);
            var items = new List<UploadedSerieItem>();
            var filesnames = model.filenameOrder.Split(',');
            var rankCounter = 0;

            foreach (var filename in filesnames)
            {
                var file = model.files.FirstOrDefault(f => f != null && f.FileName == filename);
                if (file != null)
                {
                    items.Add(new UploadedSerieItem
                    {
                        Filename = filename,
                        Stream = file.InputStream,
                        Rank = rankCounter,
                    });
                }
                else
                {
                    items.Add(new UploadedSerieItem {
                        Filename = Path.GetFileName(filename),
                        Rank = rankCounter,
                    });
                }
                rankCounter++;
            }

            _editSerieCommand.SerieId = model.SerieId;
            _editSerieCommand.SerieName = model.name;
            _editSerieCommand.SerieItems = items;
            _editSerieCommand.ProjectId = model.projectId;
            _editSerieCommand.ProjectName = model.newProjectName;

            if (model.publicationDate.HasValue())
                _editSerieCommand.PublicationDate = DateTime.Parse(model.publicationDate);

            _editSerieCommand.Handle();

            return RedirectToAction("Index", "Serie", new { area = "Admin", id = _createSerieCommand.StoredSerieId });
        }
        
        public ActionResult New()
        {
            var projects = _getProjectListOverviewQuery.Handle();

            return View(projects);
        }

        public ActionResult SubmitNew(PostedNewSerieModel model)
        {
            var items = new List<UploadedSerieItem>();
            var filesnames = model.filenameOrder.Split(',');

            foreach (var filename in filesnames)
            {
                var file = model.files.First(f => f.FileName == filename);
                items.Add(new UploadedSerieItem
                {
                    Filename= filename,
                    Stream = file.InputStream
                });
            }

            _createSerieCommand.SerieName = model.name;
            _createSerieCommand.SerieItems = items;
            _createSerieCommand.ProjectId = model.projectId;
            _createSerieCommand.ProjectName = model.newProjectName;

            if (model.publicationDate.HasValue())
                _createSerieCommand.PublicationDate = DateTime.Parse(model.publicationDate);

            _createSerieCommand.Handle();

            return RedirectToAction("Index", "Serie", new { area="Admin",  id = _createSerieCommand.StoredSerieId });
        }
    }
}