using AutoMapper;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Dto.ArticleDtos;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Dto.ArticleFieldsDto;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Dto.FieldDtos;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Dto.FieldTopicDtos;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Dto.LogDtos;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Dto.MessageDtos;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Dto.ReviewerDtos;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Dto.ReviewerFieldTopicDtos;
using GuvenliBelgeAnonimlestirmeSistemi.WebApi.Entity;

namespace GuvenliBelgeAnonimlestirmeSistemi.WebApi.Mapping
{
    public class GeneralMapping:Profile
    {
        public GeneralMapping()
        {
            CreateMap<Article, ArticleDto>().ReverseMap();
            CreateMap<Article, CreateArticleDto>().ReverseMap();

            CreateMap<Message, MessageDto>().ReverseMap();
            CreateMap<Message, CreateMessageDto>().ReverseMap();

            CreateMap<ArticleField, ArticleFieldsDto>().ReverseMap();
            CreateMap<ArticleField, CreateArticleFieldsDto>().ReverseMap();

            CreateMap<Field, FieldDto>().ReverseMap();
            CreateMap<Field, CreateFieldDto>().ReverseMap();

            CreateMap<ArticleField, ArticleFieldsDto>().ReverseMap();
            CreateMap<ArticleField, CreateArticleFieldsDto>().ReverseMap();

            CreateMap<FieldTopic, FieldTopicDto>().ReverseMap();
            CreateMap<FieldTopic, CreateFieldTopicDto>().ReverseMap();

            CreateMap<Log, LogDto>().ReverseMap();
            CreateMap<Log, CreateLogDto>().ReverseMap();

            CreateMap<ReviewerFieldTopic, ReviewerFieldTopicDto>().ReverseMap();
            CreateMap<ReviewerFieldTopic, CreateReviewerFieldTopicDto>().ReverseMap();

            CreateMap<Reviewer, ReviewerDto>().ReverseMap();
            CreateMap<Reviewer, CreateReviewerDto>().ReverseMap();
            CreateMap<ArticleField, GetArticleTopicDto>().ForMember(dest => dest.AlanAdi, opt => opt.MapFrom(src => src.AltKonular.KonuAdi)); 


        }
    }
}
