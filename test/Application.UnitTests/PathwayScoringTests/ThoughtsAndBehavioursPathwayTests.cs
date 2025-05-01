using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.ThoughtsAndBehaviours;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.PathwayScoringTests;

public class ThoughtsAndBehavioursPathwayTests
{
    [Test]
    public void ThoughtsAndBehavioursPathway_Green()
    {
        ThoughtsAndBehavioursPathway pathway = new ThoughtsAndBehavioursPathway();

        //Resilience - I tend to bounce back quickly after hard times
        pathway.G1.Answer = G1.StronglyAgree;

        //Impulsivity - I often do things without thinking of the consequences
        pathway.G2.Answer = G2.Disagree;

        //Motivation - I am really working hard to change my life
        pathway.G3.Answer = G3.StronglyAgree;

        //Hope - My life is full of problems which I can't overcome
        pathway.G4.Answer = G4.Disagree;

        //Self-esteem - I feel good about myself
        pathway.G5.Answer = G5.Agree;

        //Adaptability - I find it easy to adapt to changes in my life
        pathway.G6.Answer = G6.Neither;

        //Emotional Perception - 
        pathway.G7.Answer = G7.StronglyAgree;

        //Emotional Regulation
        pathway.G8.Answer = G8.StronglyDisagree;

        //Socail Skills
        pathway.G9.Answer = G9.StronglyAgree;

        var rag = pathway.GetRagScore(29, AssessmentLocation.NorthEastAssessmentLocation, Sex.FemaleSex);
        rag.ShouldBe(26);
    }

    [Test]
    public void ThoughtsAndBehavioursPathway_Amber()
    {
        ThoughtsAndBehavioursPathway pathway = new ThoughtsAndBehavioursPathway();

        //Resilience - I tend to bounce back quickly after hard times
        pathway.G1.Answer = G1.Disagree;

        //Impulsivity - I often do things without thinking of the consequences
        pathway.G2.Answer = G2.Disagree;

        //Motivation - I am really working hard to change my life
        pathway.G3.Answer = G3.StronglyAgree;

        //Hope - My life is full of problems which I can't overcome
        pathway.G4.Answer = G4.Disagree;

        //Self-esteem - I feel good about myself
        pathway.G5.Answer = G5.Agree;

        //Adaptability - I find it easy to adapt to changes in my life
        pathway.G6.Answer = G6.Neither;

        //Emotional Perception - 
        pathway.G7.Answer = G7.StronglyAgree;

        //Emotional Regulation
        pathway.G8.Answer = G8.Agree;

        //Socail Skills
        pathway.G9.Answer = G9.StronglyAgree;

        var rag = pathway.GetRagScore(29, AssessmentLocation.NorthEastAssessmentLocation, Sex.FemaleSex);
        rag.ShouldBe(19);
    }

    [Test]
    public void ThoughtsAndBehavioursPathway_Red()
    {
        ThoughtsAndBehavioursPathway pathway = new ThoughtsAndBehavioursPathway();

        //Resilience - I tend to bounce back quickly after hard times
        pathway.G1.Answer = G1.Disagree;

        //Impulsivity - I often do things without thinking of the consequences
        pathway.G2.Answer = G2.Agree;

        //Motivation - I am really working hard to change my life
        pathway.G3.Answer = G3.StronglyAgree;

        //Hope - My life is full of problems which I can't overcome
        pathway.G4.Answer = G4.Disagree;

        //Self-esteem - I feel good about myself
        pathway.G5.Answer = G5.Agree;

        //Adaptability - I find it easy to adapt to changes in my life
        pathway.G6.Answer = G6.StronglyAgree;

        //Emotional Perception - 
        pathway.G7.Answer = G7.Disagree;

        //Emotional Regulation
        pathway.G8.Answer = G8.Agree;

        //Socail Skills
        pathway.G9.Answer = G9.StronglyAgree;

        var rag = pathway.GetRagScore(29, AssessmentLocation.NorthEastAssessmentLocation, Sex.FemaleSex);
        rag.ShouldBe(6);
    }
}