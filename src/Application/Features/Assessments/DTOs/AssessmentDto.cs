using System.Text.Json.Serialization;
using Cfo.Cats.Application.Features.Assessments.DTOs.AssessmentModelQuestions;

namespace Cfo.Cats.Application.Features.Assessments.DTOs;

public class AssessmentDto
{
    public AssessmentPathwayDto[] Pathways =>
    [
        WorkingPathway, HousingPathway, MoneyPathway, EducationPathway, HealthAndAddictionPathway,
        RelationshipsPathway, ThoughtsAndBehavioursPathway, WellbeingAndMentalHealthPathway
    ];

    //In order of appearance in form.
    private AssessmentPathwayDto WorkingPathway { get; } =
    new()
    {
        Pathway = Pathway.Working,
        Questions =
        [
            new AssessmentToggleQuestionDto("What is your current employment status?",
                [
                    "Do not want a job", "Want a job but cannot work", "Looking for work", "In a temporary job",
                    "In a permanent job"
                ],
            "If you're in prison then think about what your most likely to be doing after release"
            ),
            new AssessmentToggleQuestionDto("When were you last in work?",
                [
                    "I have never worked", "Over a year ago", "In the last year", "I am currently working"
                ],
            "If you're in prison then think about the work you had before coming into prison"),
            new AssessmentToggleQuestionDto("Does or would your offence limit the type of work you could do?",
            [
                "Yes", "No", "Not Sure"
            ]),
            new AssessmentToggleQuestionDto("Have you volunteered / provided unpaid help to a club, group or charity in the past year?",
            [
                "No", "Yes, less than once per month", "Yes, at least once per month"
            ]),
            new AssessmentToggleQuestionDto("Do you know how to find and apply for a job should you need to?",
            [
                "Yes", "No"
            ]),
            new AssessmentToggleQuestionDto("Are you able to fill in application forms and write a CV?",
            [
                "Yes, unaided", "Yes, with help", "No"
            ]),
            new AssessmentToggleQuestionDto("Are you comfortable using computers, tablets, iPads, laptops etc?",
            [
                "Yes", "No"
            ]),
            new AssessmentToggleQuestionDto("Do you own and use a smartphone?",
                [
                    "Yes", "Yes, but I struggle to afford data", "No"
                ],
            "If you're in prison, then think about if you expect to have a smartphone on release"),
            new AssessmentToggleQuestionDto("Do you know how to disclose your offence?",
            [
                "Yes", "No"
            ]),
            new AssessmentToggleQuestionDto("Do you or your household have access to a motor vehicle?",
                ["Yes", "No"],
            "E.g. car, van, motorbike etc.")
        ]
    };

    private AssessmentPathwayDto HousingPathway { get; } =
        new()
        {
            Pathway = Pathway.Housing,
            Questions =
            [
                new AssessmentToggleQuestionDto("Where do you normally live or spend most of your time?",
                    [
                        "Sleep Rough", "Shelter, hostel, emergency housing or AP", "Temporarily staying with family or friends", "Temporary or supported housing",
                        "Housing is rented or owned by you, your partner, parent or guardian"
                    ],
                    "If you're in prison, then think about where you expect to live on release. Try to use the one option that best fits your situation"
                    ),
                new AssessmentMultipleChoiceQuestionDto("Are you facing any of the following problems? (Tick all that apply)",
                [
                    "Behind on rent / mortgage", "Facing eviction", "Having to move due to license restrictions", "Having to move due to domestic issues",
                        "Worried may become homeless", "None of the above"
                ]),
                new AssessmentMultipleChoiceQuestionDto("Do you struggle with any of the following when at home? (Tick all that apply)",
                [
                    "Cooking/ meal planning/ healthy eating", "Cleaning or looking after your home", "Keeping your home warm", "Damp, mould or condensation",
                        "Lacking essential furniture or appliances", "Lack of privacy or your own space", "Arranging utilities (gas/ electric/ phone/ broadband)",
                        "None of the above"
                ]),
                new AssessmentToggleQuestionDto("How satisfied are you with your current housing?",
                    [
                        "Very dissatisfied", "Slightly dissatisfied", "Neither satisfied or dissatisfied", "Fairly satisfied", "Very satisfied"
                    ],
                    "If you're in prison then think about your expected housing on release."
                    ),
                new AssessmentToggleQuestionDto("How safe do/would you feel walking alone in your local area after dark?",
                [
                    "Very unsafe", "A bit unsafe", "Fairly safe", "Very safe"
                ]),
                new AssessmentMultipleChoiceQuestionDto("Do you live with any of the following? (tick all that apply)",
                [
                    "Partner/spouse", "Own children", "Other children", "Other family members",
                        "Other non-family members", "Live alone"
                ])
            ]
        };

    private AssessmentPathwayDto MoneyPathway { get; } = new()
    {
        Pathway = Pathway.Money,
        Questions =
        [
            new AssessmentToggleQuestionDto("How are you coping with money?",
                [
                    "Finding it very difficult", "Finding it quite difficult", "Just about getting by", "Doing alright", "Living comfortably"
                ],
                "If you're in prison, then think about the time before you cam into prison"
                ),
            new AssessmentToggleQuestionDto("Are you behind on credit card or store card payments or in paying your household bills?",
            [
                "Yes", "No"
            ]),
            new AssessmentToggleQuestionDto("Do you owe over £1000 in unsecured loans, credit cards or other debt?",
                [
                    "Yes", "No"
                ],
                "This includes things such as court fines but doesn't include mortgages or secured loans"
                ),
            new AssessmentToggleQuestionDto("Do you owe any informal debt?",
                [
                    "Yes", "No"
                ],
                "This includes owing money to loan sharks, prison debt, drug debt or owing family/friends."
                ),
            new AssessmentToggleQuestionDto("Do you have a bank account?",
                ["Yes", "No"]
            ),
            new AssessmentToggleQuestionDto("Do you have a valid ID? (e.g. passport)",
            [
                "Yes", "No"
            ]),
            new AssessmentToggleQuestionDto("Do you need help with budgeting or managing your money?",
            [
                "Yes", "No"
            ]),
            new AssessmentToggleQuestionDto("Do you need help or support with beneifts?",
            [
                "Yes", "No"
            ]),
            new AssessmentToggleQuestionDto("Have you used a food bank?",
                [
                    "Never", "Over a year ago", "Within the last year", "Within the last 30 days"
                ],
                "If you're in prison, think about the time before you came into prison"
                )
        ]
    };

    private AssessmentPathwayDto EducationPathway { get; } = new()
    {
        Pathway = Pathway.Education,
        Questions =
        [
            new AssessmentToggleQuestionDto("What is your highest level of qualification?",
                [
                    "No quals/ entry level", "NVQ level 1 or similar", "5+ GCSEs, NVQ level 2", "2+ A-levels, NVQ level 3, apprenticeship, etc", "Degree or higher"
                ]
                ,
                "This could be how far you got in school or college or could be a qualification you got later."
                ),
            new AssessmentToggleQuestionDto("Did you finish school?",
            [
                "Yes", "Left before age 16", "Left before age 11"
            ]),
            new AssessmentMultipleChoiceQuestionDto("Have you been diagnosed with or feel you may have any of the following? (tick all that apply)",
                [
                    "Autism/ ASD", "ADHD/ADD", "Epilepsy", "Synaesthasia", "Tourette syndrome", "An intellectual disability", "Dyslexia", "Dyspraxia",
                    "Other learning disability", "None of these"
                ],
                "It is ok to tick if you have not been formally diagnosed by a doctor yet if you feel you have it."
                ),
            new AssessmentMultipleChoiceQuestionDto("Do you have difficulty with any of the following? (tick all that apply)",
                [
                    "Reading", "Coordination", "Writing", "Speaking", "Using numbers", "Concentrating", "Memory", "Understanding others", "Restlessness/ sitting still", "None of these"
                ],
                "Try to think if they affect your day-to-day life in a negative way."
                ),
            new AssessmentMultipleChoiceQuestionDto("In the last 12 months have you participated in any of the following creative activities? (tick all that apply)",
            [
                "Painting", "Making films", "Drawing", "Photography", "Crafts", "Performing music", "Writing stories/poetry", "Performing drama", "Reading",
                    "Puzzles", "Other creative activity", "None of these"
            ])
        ]
    };

    private AssessmentPathwayDto HealthAndAddictionPathway { get; } = new()
    {
        Pathway = Pathway.HealthAndAddiction,
        Questions =
        [
            new AssessmentToggleQuestionDto("How is your health in general?",
            [
                "Very bad", "Bad", "Fair", "Good", "Very good"
            ]),
            new AssessmentToggleQuestionDto("Do you consider yourself disabled?",
                [
                    "No", "Yes, a little impairment", "Yes, a lot of impairment"
                ],
                "For example, do you have a long-term physical or mental health condition/illness that reduces your ability to carry-out day-to-day activities?"),
            new AssessmentToggleQuestionDto("Are you currently taking any regular medication or undergoing treatment for a physical or mental health condition?",
            [
                "No", "Yes", "Not Sure"
            ]),
            new AssessmentToggleQuestionDto("Are you registered with a GP and dentist?",
            [
                "Registered with both", "Registered with a GP only", "Registered with a dentist only", "Not registered with either"
            ]),
            new AssessmentToggleQuestionDto("How much sport, exercise or physical activity do you do in a typical week?",
                [
                    "Less than 30min per week", "30min to 2 1/2 hours per week", "Over 2 1/2 hours per week"
                ],
                "This can be anything that raises your heart rate or gets you out of breath"),
            new AssessmentMultipleChoiceQuestionDto("Do you have a problem, receive, or require help with any of the following? (tick all that apply)",
                [
                    "Alcohol", "Gambling", "Illegal drugs / substances", "Sex or pornography", "Prescription drugs", "Food", "Something else", "None"
                ],
                "Illegal drugs include substances such as cannabis, spice, heroin and cocaine. Prescription drugs include substances such as methadone, painkillers, " +
                "antidepressants, benzos and sleeping pills."),
            new AssessmentMultipleChoiceQuestionDto("Have you previously had a problem or received help with any of the following? (tick all that apply)",
                [
                    "Alcohol", "Gambling", "Illegal drugs / substances", "Sex or pornography", "Prescription drugs", "Food", "Something else", "None"
                ],
                "See above above for guidance"),
            new AssessmentToggleQuestionDto("Are you a smoker?",
                ["Yes", "I used to", "Never smoked"]),
            new AssessmentToggleQuestionDto("Are you a vaper?",
            [
                "Yes", "I used to", "Never vaped"
            ])
        ]
    };

    private AssessmentPathwayDto RelationshipsPathway { get; } = new()
    {
        Pathway = Pathway.Relationships,
        Questions =
        [
            new AssessmentToggleQuestionDto("How happy are you with your current personal relationships?",
            [
                "Extremely unhappy", "Fairly unhappy", "Happy", "Very happy", "Extremely happy"
            ]),
            new AssessmentToggleQuestionDto("How often do you feel lonely?",
            [
                "Often or always", "Some of the time", "Occasionally", "Hardly ever", "Never"
            ]),
            new AssessmentToggleQuestionDto("Are you interested in having a mentor?",
                [
                    "Yes", "No", "Not sure"
                ],
                "For example, someone to give you support, advice and guidance to help you."),
            new AssessmentToggleQuestionDto("Do you feel you could be a suitable mentor for someone else?",
            [
                "Yes", "No", "Not sure"
            ]),
            new AssessmentToggleQuestionDto("How often do you see family or close friends in person?",
            [
                "At least once per week", "Less than once per week", "Rarely or never"
            ]),
            new AssessmentToggleQuestionDto("On a scale 1 to 5, in general how much do you trust most people?",
            [
                "Not at all", "A little", "Somewhat", "Mostly", "Completely"
            ]),
            new AssessmentMultipleChoiceQuestionDto("Do you feel you can trust, confide in, and rely on any of the following? (tick all that apply)",
            [
                "A partner/ spouse", "A close freind", "A parent/ guardian", "A case/ support worker", "Other family member", "None of these"
            ]),
            new AssessmentToggleQuestionDto("Are you a carer?",
                [
                    "No", "Yes, under 10 hrs per week", "Yes, 10-34 hrs per week", "Yes, 35-49 hrs per week", "Yes, 50+ hrs per week"
                ],
                "For example, do you provide unpaid care for family or a friend who needs help due to their illness, disability, " +
                "mental health problem or an addiction? This can be officially or unofficially."),
            new AssessmentToggleQuestionDto("Which of the following best describes your parental situation?",
                [
                    "I do not have children", "I have children but I am not responsible for their care", "My children are adults/ have left home",
                    "I share their care with someone else (e.g. your partner)", "I consider myself a lone parent"
                ],
                "Try to select the option which best describes your situation.")
        ]
    };

    private AssessmentPathwayDto ThoughtsAndBehavioursPathway { get; } = new()
    {
        Pathway = Pathway.ThoughtsAndBehaviours,
        Questions =
        [
            new AssessmentToggleQuestionDto("I tend to bounce back quickly after hard times",
            [
                "Strongly disagree", "Disagree", "Neither", "Agree", "Strongly agree"
            ]),
            new AssessmentToggleQuestionDto("I often do things without thinking of the consequences",
            [
                "Strongly disagree", "Disagree", "Neither", "Agree", "Strongly agree"
            ]),
            new AssessmentToggleQuestionDto("I am really working hard to change my life",
            [
                "Strongly disagree", "Disagree", "Neither", "Agree", "Strongly agree"
            ]),
            new AssessmentToggleQuestionDto("My life is full of problems which I can't overcome",
            [
                "Strongly disagree", "Disagree", "Neither", "Agree", "Strongly agree"
            ]),
            new AssessmentToggleQuestionDto("I feel good about myself",
            [
                "Strongly disagree", "Disagree", "Neither", "Agree", "Strongly agree"
            ]),
            new AssessmentToggleQuestionDto("I find it easy to adapt to changes in my life",
            [
                "Strongly disagree", "Disagree", "Neither", "Agree", "Strongly agree"
            ]),
            new AssessmentToggleQuestionDto("I struggle to understand my own or other people's feelings",
            [
                "Strongly disagree", "Disagree", "Neither", "Agree", "Strongly agree"
            ]),
            new AssessmentToggleQuestionDto("My emotions can get the better of me",
            [
                "Strongly disagree", "Disagree", "Neither", "Agree", "Strongly agree"
            ]),
            new AssessmentToggleQuestionDto("I find it easy to talk to other people",
            [
                "Strongly disagree", "Disagree", "Neither", "Agree", "Strongly agree"
            ])
        ]
    };

    private AssessmentPathwayDto WellbeingAndMentalHealthPathway { get; } = new()
    {
        Pathway = Pathway.WellbeingAndMentalHealth,
        Questions =
        [
            new AssessmentToggleQuestionDto("I have felt tense, anxious or nervous",
            [
                "Not at all", "Only occasionally", "Sometimes", "Often", "Most or all of the time"
            ]),
            new AssessmentToggleQuestionDto("I have felt able to cope when things go wrong",
            [
                "Not at all", "Only occasionally", "Sometimes", "Often", "Most or all of the time"
            ]),
            new AssessmentToggleQuestionDto("Talking to people has felt too much for me",
            [
                "Not at all", "Only occasionally", "Sometimes", "Often", "Most or all of the time"
            ]),
            new AssessmentToggleQuestionDto("I have felt panic or terror",
            [
                "Not at all", "Only occasionally", "Sometimes", "Often", "Most or all of the time"
            ]),
            new AssessmentToggleQuestionDto("I made plans to end my life",
            [
                "Not at all", "Only occasionally", "Sometimes", "Often", "Most or all of the time"
            ]),
            new AssessmentToggleQuestionDto("I have had difficulty getting to sleep or staying asleep",
            [
                "Not at all", "Only occasionally", "Sometimes", "Often", "Most or all of the time"
            ]),
            new AssessmentToggleQuestionDto("I have felt despairing or helpless",
            [
                "Not at all", "Only occasionally", "Sometimes", "Often", "Most or all of the time"
            ]),
            new AssessmentToggleQuestionDto("I have felt unhappy",
            [
                "Not at all", "Only occasionally", "Sometimes", "Often", "Most or all of the time"
            ]),
            new AssessmentToggleQuestionDto("Unwanted images or memories have been distressing me",
            [
                "Not at all", "Only occasionally", "Sometimes", "Often", "Most or all of the time"
            ]),
            new AssessmentToggleQuestionDto("I have felt I have someone to turn to for support when needed",
            [
                "Not at all", "Only occasionally", "Sometimes", "Often", "Most or all of the time"
            ]),
            new AssessmentToggleQuestionDto("On a scale of 1 to 5, overall how satisfied are you with your life",
            [
                "1- Not at all", "2- Mostly not", "3- Fairly", "4- Mostly", "5- completely"
            ]),
            new AssessmentToggleQuestionDto("How often do you feel stressed",
            [
                "Every day", "1-2 times a week", "Few times a month", "Rarely or never"
            ]),
            new AssessmentMultipleChoiceQuestionDto("Do you live with any of the following mental health disorders",
            [
                "Anxiety disorders", "Bipolar disorder", "Borderline personality disorders",
                    "Depression", "Eating disorders", "Panic disorder", "Obsessive compulsive disorder",
                    "Psychosis", "Personality disorders", "Schizophrenia", "Post-traumatic stress disorder",
                    "Self-harm", "Other", "None of these"
            ])
        ]
    };
}
