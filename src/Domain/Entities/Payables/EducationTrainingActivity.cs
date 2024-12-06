﻿using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Domain.Entities.Payables;

public class EducationTrainingActivity : Activity
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    EducationTrainingActivity()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    EducationTrainingActivity(
        ActivityDefinition definition,
        string participantId,
        Location tookPlaceAtLocation,
        Contract tookPlaceAtContract,
        Location participantCurrentLocation,
        Contract? participantCurrentContract,
        EnrolmentStatus participantStatus,
        string? additionalInformation,
        DateTime completed,
        string tenantId) : base(definition, participantId, tookPlaceAtLocation, tookPlaceAtContract, participantCurrentLocation, participantCurrentContract, participantStatus, additionalInformation, completed, tenantId) 
    {
        AddDomainEvent(new EducationTrainingActivityCreatedDomainEvent(this));
    }

    /*
    public string CourseTitle { get; private set; }
    public string CourseUrl { get; private set; }
    public string CourseLevel { get; private set; }
    public DateTime CourseCommenced { get; private set; }
    public Datetime CourseCompleted { get; private set; }
    public bool Passed { get; private set; } // Needs to be enum: Passed, Failed, Not Applicable
    public Document Document { get; private set; } // Uploaded template
    */

    public static EducationTrainingActivity Create(
        ActivityDefinition definition,
        string participantId,
        Location tookPlaceAtLocation,
        Contract tookPlaceAtContract,
        Location participantCurrentLocation,
        Contract? participantCurrentContract,
        EnrolmentStatus participantStatus,
        string? additionalInformation,
        DateTime completed,
        string tenantId)
    {
        EducationTrainingActivity activity = new(
            definition, 
            participantId, 
            tookPlaceAtLocation, 
            tookPlaceAtContract, 
            participantCurrentLocation, 
            participantCurrentContract, 
            participantStatus, 
            additionalInformation, 
            completed, 
            tenantId);

        return activity;
    }

}