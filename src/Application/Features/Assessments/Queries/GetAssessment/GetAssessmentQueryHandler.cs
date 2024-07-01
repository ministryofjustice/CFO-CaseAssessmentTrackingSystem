using Cfo.Cats.Application.Features.Assessments.DTOs;

namespace Cfo.Cats.Application.Features.Assessments.Queries.GetAssessment;

public class GetAssessmentQueryHandler : IRequestHandler<GetAssessmentQuery, Result<AssessmentDto>>
{

    public async Task<Result<AssessmentDto>> Handle(GetAssessmentQuery request, CancellationToken cancellationToken)
    {
        AssessmentBuilder builder = new AssessmentBuilder();

        builder.AddPathway(Pathway.Working)
            .WithToggleChoiceQuestion("What is your current employment status?",
            "If you’re in prison, then think about what you expect to be doing after release.",
            [
                "Do not want a job",
                "Want a job but cannot work",
                "Looking for work",
                "In a temporary job",
                "In a permanent job"
            ])
            .WithToggleChoiceQuestion("When were you last in work?",
            "If you’re in prison, then think about any work you had before coming into prison.",
            [
                "I have never worked",
                "Over a year ago",
                "In the last year",
                "I am currently working"
            ])
            .WithToggleChoiceQuestion("Does or would your offence limit the types of work you could do?",
            [
                "Yes",
                "No",
                "Not Sure"
            ])
            .WithToggleChoiceQuestion("Have you volunteered / provided unpaid help to a club, group, or charity in the last year?",
            [
                "No",
                "Yes, less than once per month",
                "Yes, at least once per month"
            ])
            .WithYesNoQuestion("Do you know how to find and apply for a job should you need to?")
            .WithToggleChoiceQuestion("Are you able to fill in application forms and write a CV?",
            [
                "Yes, unaided",
                "Yes, with help",
                "No"
            ])
            .WithYesNoQuestion("Are you comfortable using computers, tablets, iPads, laptops etc?")
            .WithToggleChoiceQuestion("Do you own and use a smart phone?", "If you’re in prison, then think about whether you expect to have a smart phone on release.",
            [
                "Yes",
                "Yes, but I struggle to afford data",
                "No"
            ])
            .WithYesNoQuestion("Do you know how to disclose your offence?")
            .WithYesNoQuestion("Do you or your household have access to a motor vehicle?", "E.g., a car, van, motorbike, moped etc.")
            .AddPathway(Pathway.Housing)
            .WithToggleChoiceQuestion("Where do you normally live or spend most of your time?", "If you’re in prison, then think about where you expect to live on release. Try to choose the one option that best fits your situation.",
            [
                "Sleep rough",
                "Shelter, hostel, emergency housing or AP",
                "Temporarily staying with family or friends",
                "Temporary or supported housing",
                "Housing is rented or owned by you, your partner, parent or guardian"
            ])
            .WithMultipleChoiceQuestion("Are you facing any of the following problems? (tick all that apply)",
            [
                "Behind on rent/mortgage",
                "Facing eviction",
                "Having to move due to licence restrictions",
                "Having to move due to domestic issues",
                "Worried may become homeless",
                "None of the above"
            ])
            .WithMultipleChoiceQuestion("Do you struggle with any of the following when at home? (tick all that apply)",
            [
                "Cooking/meal planning/healthy eating",
                "Cleaning or looking after your home",
                "Keeping your home warm",
                "Damp, mould, or condensation",
                "Lacking essential furniture or appliances",
                "Lack of privacy or your own space",
                "Arranging utilities (gas/electric/phone/broadband etc.)",
                "None of the above"
            ])
            .WithToggleChoiceQuestion("How satisfied are you with your current housing?", "If you’re in prison, then think about your expected housing on release.",
            [
                "Very dissatisfied",
                "Slightly dissatisfied",
                "Neither satisfied or dissatisfied",
                "Fairly satisfied",
                "Very satisfied",
            ])
            .WithToggleChoiceQuestion("How safe do/would you feel walking alone in your local area after dark?",
            [
                "Very unsafe",
                "A bit unsafe",
                "Fairly safe",
                "Very safe",
            ])
            .WithMultipleChoiceQuestion("Do you live with any of the following? (tick all that apply)", "If you’re in prison, then think about who you expect to live with on release.",
            [
                "Partner/spouse",
                "Other family members",
                "Own Children",
                "Other non-family members",
                "Other Children",
                "Live alone"
            ])
            .AddPathway(Pathway.Money)
            .WithToggleChoiceQuestion("How are you coping with money?", "If you’re in prison, then think about the time before you came into prison.",
            [
                "Finding it very difficult",
                "Finding it quite difficult",
                "Just about getting by",
                "Doing alright",
                "Living comfortably"
            ])
            .WithYesNoQuestion("Are you behind on credit card or store card payments or in paying your household bills?")
            .WithYesNoQuestion("Do you owe over £1000 in unsecured loans, credit cards or other debt?", "This includes things such as court fines but doesn’t include mortgages or secured loans.")
            .WithYesNoQuestion("Do you owe any informal debt?", "This includes owing money to loan sharks, prison debt, drug debt or owing family/friends.")
            .WithYesNoQuestion("Do you have a bank account?")
            .WithYesNoQuestion("Do you have a valid ID? (e.g. passport)")
            .WithYesNoQuestion("Do you need help with budgeting or managing your money?")
            .WithYesNoQuestion("Do you need help or support with benefits?")
            .WithToggleChoiceQuestion("Have you used a food bank?",
            "If you’re in prison, think about the time before you came into prison.",
            [
                "Never",
                "Over a year ago",
                "Within the last year",
                "Within the last 30 days"
            ])
            .AddPathway(Pathway.Education)
            .WithToggleChoiceQuestion("What is your highest level of qualification?",
            "This could be how far you got in school/college or could be a qualification you got later.",
            [
                "No quals / entry level Example:Skills for life",
                "NVQ level 1 or similar Example:Grade D/3 or lower at GCSE",
                "5+ GCSEs, NVQ level 2, etc. Example:Grade C/4 or higher at GCSE",
                "2+ A-levels, NVQ level 3, apprentice-ship, etc. Example:HNC",
                "Degree or higher Example:BA"
            ])
            .WithToggleChoiceQuestion("Did you finish school?", [
                "Yes",
                "Left before age 16",
                "Left before age 11"
            ])
            .WithMultipleChoiceQuestion("Have you been diagnosed with or feel you may have any of the following? (tick all that apply)",
            "It is ok to tick if you have not been formally diagnosed by a doctor yet if you feel you have it.",
            [
                "Autism / ASD",
                "ADHD / ADD",
                "Epilepsy",
                "Synaesthesia",
                "Tourette syndrome",
                "An intellectual disability",
                "Dyslexia",
                "Dyspraxia",
                "Dyscalculia",
                "Dysgraphia",
                "Other learning disability",
                "None of these"
            ])
            .WithMultipleChoiceQuestion("Do you have difficulty with any of the following? (tick all that apply)",
            "Try to think if they affect your day-to-day life in a negative way.",
            [
                "Reading",
                "Coordination",
                "Writing",
                "Speaking",
                "Using numbers",
                "Concentrating",
                "Memory",
                "Understanding others",
                "Restlessness / sitting still",
                "Others"
            ]
            )
            .WithMultipleChoiceQuestion("In the last 12 months have you participated in any of the following creative activities? (tick all that apply)",
            [
                "Painting",
                "Making films",
                "Drawing",
                "Photography",
                "Crafts",
                "Performing music",
                "Writing stories/poetry",
                "Performing drama",
                "Reading",
                "Puzzles",
                "Other creative activity",
                "None of these"
            ])
            .AddPathway(Pathway.HealthAndAddiction)
            .WithToggleChoiceQuestion("How is your health in general?", [
                "Very bad",
                "Bad",
                "Fair",
                "Good",
                "Very good",
            ])
            .WithToggleChoiceQuestion("Do you consider yourself disabled?", "For example, do you have a long-term physical or mental health condition/illness that reduces your ability to carry-out day-to-day activities?",
            [
                "No",
                "Yes, A little impairment",
                "Yes, A lot of impairment"
            ])
            .WithToggleChoiceQuestion("Are you currently taking any regular medication or undergoing treatment for a physical or mental health condition?",
            [
                "Yes",
                "No",
                "Not sure"
            ])
            .WithToggleChoiceQuestion("Are you registered with a GP and dentist?",
            [
                "Registered with both",
                "Registered with a GP only",
                "Registered with a dentist only",
                "Not registered with either",
            ])
            .WithToggleChoiceQuestion("How much sport, exercise or physical activity do you do in a typical week?",
            "This can be anything that raises your heart-rate or gets you out of breath.",
            [
                "Less than 30min per week",
                "30min to 2½ hr per week",
                "Over 2½ hrs per week",
            ])
            .WithMultipleChoiceQuestion("Do you have a problem, receive, or require help with any of the following? (tick all that apply)",
            "Illegal drugs include substances such as cannabis, spice, heroin, and cocaine. Prescription drugs include substances such as methadone, painkillers, antidepressants, benzos and sleeping pills.",
            [
                "Alcohol",
                "Gambling",
                "Illegal drugs / substances",
                "Sex or pornography",
                "Prescription drugs",
                "Food",
                "Something else",
                "None"
            ])
            .WithMultipleChoiceQuestion("Have you previously had a problem or received help with any of the following? (tick all that apply)",
            "See above for guidance.",
            [
                "Alcohol",
                "Gambling",
                "Illegal drugs / substances",
                "Sex or pornography",
                "Prescription drugs",
                "Food",
                "Something else",
                "None"
            ])
            .WithToggleChoiceQuestion("Are you a smoker?",
            [
                "Yes",
                "I used to",
                "Never smoked"
            ])
            .WithToggleChoiceQuestion("Are you a vaper?",
            [
                "Yes",
                "I used to",
                "Never vaped"
            ])
            .AddPathway(Pathway.Relationships)
            .WithToggleChoiceQuestion("How happy are you with your current personal relationships?",
            [
                "Extremely unhappy",
                "Fairly unhappy",
                "Happy",
                "Very happy",
                "Extremely happy",
            ])
            .WithToggleChoiceQuestion("How often do you feel lonely?",
            [
                "Often or always",
                "Some of the time",
                "Occasionally",
                "Hardly ever",
                "Never",
            ])
            .WithToggleChoiceQuestion("Are you interested in having a mentor?", "For example, someone to give you support, advice, and guidance to help you.",
            [
                "Yes",
                "No",
                "Not sure",
            ])
            .WithToggleChoiceQuestion("Do you feel you could be a suitable mentor for someone else?",
            [
                "Yes",
                "No",
                "Not sure",
            ])
            .WithToggleChoiceQuestion("How often do you see family or close friends in person?",
            [
                "At least once per week",
                "Less than once per week",
                "Rarely or never",
            ])
            .WithToggleChoiceQuestion("On a scale 1 to 5, in general how much do you trust most people?",
            [
                "1 Not at all",
                "2 A little",
                "3 Somewhat",
                "4 Mostly",
                "5 Completely",
            ])
            .WithToggleChoiceQuestion("Do you feel you can trust, confide in, and rely on any of the following? (tick all that apply)",
            [
                "A partner / spouse",
                "A close friend",
                "A parent / guardian",
                "A case/support worker",
                "Other family member",
                "None of these",
            ])
            .WithToggleChoiceQuestion("Are you a carer?", "For example, do you provide unpaid care for family or a friend who needs help due to their illness, disability, mental health problem or an addiction? This can be officially or unofficially.",
            [
                "No",
                "Yes - Under 10 hrs per week",
                "Yes - 10 – 34 hrs per week",
                "Yes - 35 - 49 hrs per week",
                "Yes - 50+ hrs per week"
            ])
            .WithToggleChoiceQuestion("Which of the following best describes your parental situation?",
            "Try to select the option which best describes your situation.",
            [
                "I do not have children",
                "I have children but I am not responsible for their care",
                "My children are adults / have left home",
                "I share their care with someone else (e.g., your partner)",
                "I consider myself a lone parent",
            ])
            .AddPathway(Pathway.ThoughtsAndBehaviours)
            .WithAgreementQuestion("I tend to bounce back quickly after hard times")
            .WithAgreementQuestion("I often do things without thinking of the consequences")
            .WithAgreementQuestion("I am really working hard to change my life")
            .WithAgreementQuestion("My life is full of problems which I can't overcome")
            .WithAgreementQuestion("I feel good about myself")
            .WithAgreementQuestion("I find it easy to adapt to changes in my life")
            .WithAgreementQuestion("I struggle to understand my own or other people's feelings")
            .WithAgreementQuestion("My emotions can get the better of me")
            .WithAgreementQuestion("I find it easy to talk to other people")
            .WithMultipleChoiceQuestion("Which of the following is/are the most important to you?",
            "You can select more than one option if you want to, but you must select at least one.",
            [
                "Your offending",
                "Learning & education",
                "Your wellbeing & mental health",
                "Your housing situation",
                "Your physical health",
                "Your financial situation",
                "An addiction",
                "Getting into or keeping work",
                "Your thoughts and behaviour",
                "Something else",
                "Your relationships, family, friends",
                "Nothing"
            ])
            .AddPathway(Pathway.WellbeingAndMentalHealth)
            .WithFeelingQuestion("Talking to people has felt too much for me")
            .WithFeelingQuestion("I have felt panic or terror")
            .WithFeelingQuestion("I made plans to end my life")
            .WithFeelingQuestion("I have had difficulty getting to sleep or staying asleep")
            .WithFeelingQuestion("I have felt despairing or helpless")
            .WithFeelingQuestion("I have felt unhappy")
            .WithFeelingQuestion("Unwanted images or memories have been distressing me")
            .WithFeelingQuestion("I have felt I have someone to turn to for support when needed")
            .WithToggleChoiceQuestion("On a scale 1 to 5, overall how satisfied are you with your life?", [
                "1 Not at all",
                "2 Mostly not",
                "3 Fairly",
                "4 Mostly",
                "5 Completely",
            ])
            .WithToggleChoiceQuestion("How often do you feel stressed?",
            [
                "Every day",
                "1-2 times a week",
                "Few times a month",
                "Rarely or never",
            ])
            .WithMultipleChoiceQuestion("Do you live with any of the following mental health conditions? (tick all that apply)",
            "It is ok to tick if you have not been formally diagnosed by a doctor yet if you feel you have it",
            [
                "Anxiety disorders",
                "Bipolar Disorder",
                "Borderline Personality Disorder",
                "Depression",
                "Eating Disorders",
                "Panic Disorder",
                "Obsessive Compulsive Disorder",
                "Psychosis",
                "Personality Disorders",
                "Schizophrenia",
                "Post-Traumatic Stress Disorder",
                "Self-harm",
                "Other",
                "None of these",
            ]);
            ;


        return await Result<AssessmentDto>.SuccessAsync(builder.Build());
    }
}
